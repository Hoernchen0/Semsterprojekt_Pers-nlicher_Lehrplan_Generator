using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.ViewModels.Main;
using LehrplanGenerator.Models.Chat;
using LehrplanGenerator.Logic.AI;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;


namespace LehrplanGenerator.ViewModels.Chat;

public partial class ChatViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly AppState _appState;
    private readonly StudyPlanGeneratorService _studyPlanGeneratorService;
    private readonly LearningProgressService _learningProgressService;

    public ChatViewModel(
        INavigationService navigationService, 
        AppState appState,
        StudyPlanGeneratorService studyPlanGeneratorService,
        LearningProgressService learningProgressService)
    {
        _navigationService = navigationService;
        _appState = appState;
        _studyPlanGeneratorService = studyPlanGeneratorService;
        _learningProgressService = learningProgressService;

        Messages.CollectionChanged += OnMessagesChanged;
    }

    // =========================
    // MESSAGES
    // =========================
    public ObservableCollection<ChatMessage> Messages => _appState.ChatMessages;

    public bool HasMessages => Messages.Count > 0;
    public bool HasNoMessages => Messages.Count == 0;

    private void OnMessagesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(HasMessages));
        OnPropertyChanged(nameof(HasNoMessages));
    }

    // =========================
    // INPUT
    // =========================
    [ObservableProperty]
    private string inputText = string.Empty;

    [ObservableProperty]
    private bool isSending;

    // =========================
    // COMMANDS
    // =========================

    [RelayCommand]
    private async Task Upload()
    {
        try
        {
            // StorageProvider vom Hauptfenster holen
            var lifetime = Avalonia.Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var mainWindow = lifetime?.MainWindow;
            
            if (mainWindow == null)
            {
                Messages.Add(new ChatMessage
                {
                    Sender = "System",
                    FullText = "Fehler: Hauptfenster nicht verfügbar.",
                    DisplayedText = "Fehler: Hauptfenster nicht verfügbar."
                });
                return;
            }

            // File-Picker konfigurieren
            var options = new FilePickerOpenOptions
            {
                Title = "PDF-Datei auswählen",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("PDF-Dateien")
                    {
                        Patterns = new[] { "*.pdf" },
                        MimeTypes = new[] { "application/pdf" }
                    }
                }
            };

            // Datei-Dialog öffnen
            var files = await mainWindow.StorageProvider.OpenFilePickerAsync(options);
            
            if (files.Count == 0)
            {
                // Benutzer hat abgebrochen
                return;
            }

            var selectedFile = files[0];
            var filePath = selectedFile.Path.LocalPath;

            // PDF an AI-Service übergeben
            var success = await _appState.AiService.UploadPdfAsync(filePath);

            if (success)
            {
                Messages.Add(new ChatMessage
                {
                    Sender = "System",
                    FullText = $"✓ PDF '{selectedFile.Name}' erfolgreich hochgeladen und verarbeitet!",
                    DisplayedText = $"✓ PDF '{selectedFile.Name}' erfolgreich hochgeladen und verarbeitet!"
                });
            }
            else
            {
                Messages.Add(new ChatMessage
                {
                    Sender = "System",
                    FullText = $"❌ Fehler beim Verarbeiten der PDF '{selectedFile.Name}'.",
                    DisplayedText = $"❌ Fehler beim Verarbeiten der PDF '{selectedFile.Name}'."
                });
            }
        }
        catch (Exception ex)
        {
            Messages.Add(new ChatMessage
            {
                Sender = "System",
                FullText = $"Fehler beim Hochladen: {ex.Message}",
                DisplayedText = $"Fehler beim Hochladen: {ex.Message}"
            });
        }
    }

    [RelayCommand]
    private async Task Send()
    {
        if (IsSending)
            return;

        if (string.IsNullOrWhiteSpace(InputText))
            return;

        var userText = InputText.Trim();
        InputText = string.Empty;

        Messages.Add(new ChatMessage
        {
            Sender = "User",
            FullText = userText,
            DisplayedText = userText
        });

        IsSending = true;

        try
        {
            var response = await _appState.AiService.AskGptAsync(userText);

            if (!string.IsNullOrWhiteSpace(response))
            {
                var aiMessage = new ChatMessage
                {
                    Sender = "AI",
                    FullText = response,
                    DisplayedText = ""
                };

                Messages.Add(aiMessage);
                await TypeTextAsync(aiMessage);
            }
        }
        finally
        {
            IsSending = false;
        }
    }

    // =========================
    // TYPING EFFECT
    // =========================
    private async Task TypeTextAsync(ChatMessage message)
    {
        var sb = new StringBuilder();

        foreach (var ch in message.FullText)
        {
            sb.Append(ch);
            message.DisplayedText = sb.ToString();
            await Task.Delay(15);
        }
    }

    // =========================
    // NAVIGATION
    // =========================
    [RelayCommand]
    private void GoBack()
    {
        _navigationService.NavigateTo<MainViewModel>();
    }

    // =========================
    // CREATE STUDY PLAN
    // =========================
    [ObservableProperty]
    private bool isCreatingPlan;

    [RelayCommand]
    private async Task CreateNewPlan()
    {
        if (IsCreatingPlan)
            return;

        if (_appState.CurrentUserId == null)
        {
            Messages.Add(new ChatMessage
            {
                Sender = "System",
                FullText = "Fehler: Kein Benutzer angemeldet!",
                DisplayedText = "Fehler: Kein Benutzer angemeldet!"
            });
            return;
        }

        IsCreatingPlan = true;

        try
        {
            Console.WriteLine($"📋 Starte KI-Lernplan-Generierung aus Chat...");

            // Nutze den Chat-Kontext: Nimm die letzte User-Nachricht als Prompt
            string prompt = "Erstelle einen Lernplan für Softwareentwicklung. " +
                "Plane die nächsten 5-7 Tage mit jeweils 3-4 Lerneinheiten à 50 Minuten mit 10 Minuten Pausen. " +
                "Das Studium beginnt morgen.";

            // Wenn es aktuelle Chat-Messages gibt, nutze die letzte User-Nachricht
            if (Messages.Count > 0)
            {
                var lastUserMessage = Messages.LastOrDefault(m => m.Sender == "User");
                if (lastUserMessage != null)
                {
                    prompt = lastUserMessage.FullText;
                    Console.WriteLine($"📝 Nutze Chat-Kontext: {prompt}");
                }
            }

            // Frage KI um einen Plan zu erstellen
            var planResponse = await _studyPlanGeneratorService.AskGptAsync(prompt);

            if (string.IsNullOrEmpty(planResponse))
            {
                Messages.Add(new ChatMessage
                {
                    Sender = "System",
                    FullText = "Fehler bei der KI-Generierung",
                    DisplayedText = "Fehler bei der KI-Generierung"
                });
                return;
            }

            Console.WriteLine($"🤖 KI hat Lernplan geplant");

            // Generiere den StudyPlan
            var studyPlan = await _studyPlanGeneratorService.CreateStudyPlanAsync();

            if (studyPlan == null)
            {
                Messages.Add(new ChatMessage
                {
                    Sender = "System",
                    FullText = "Fehler beim Generieren des Lernplans",
                    DisplayedText = "Fehler beim Generieren des Lernplans"
                });
                return;
            }

            Console.WriteLine($"📊 StudyPlan generiert mit {studyPlan.Days.Count} Tagen");

            // Speichere den Plan in der Datenbank
            await _learningProgressService.SaveStudyPlanAsync(_appState.CurrentUserId.Value, studyPlan);

            Console.WriteLine($"✅ Lernplan gespeichert: {studyPlan.Topic}");

            Messages.Add(new ChatMessage
            {
                Sender = "System",
                FullText = $"✅ Lernplan erfolgreich erstellt!\n📚 Thema: {studyPlan.Topic}\n📅 Tage: {studyPlan.Days.Count}\n\nDu kannst den Plan jetzt in der Lernplan-View sehen.",
                DisplayedText = $"✅ Lernplan erfolgreich erstellt!\n📚 Thema: {studyPlan.Topic}\n📅 Tage: {studyPlan.Days.Count}\n\nDu kannst den Plan jetzt in der Lernplan-View sehen."
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Fehler beim Erstellen des Lernplans: {ex.Message}\n{ex.StackTrace}");
            Messages.Add(new ChatMessage
            {
                Sender = "System",
                FullText = $"Fehler: {ex.Message}",
                DisplayedText = $"Fehler: {ex.Message}"
            });
        }
        finally
        {
            IsCreatingPlan = false;
        }
    }
}