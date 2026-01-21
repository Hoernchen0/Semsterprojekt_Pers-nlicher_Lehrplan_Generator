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
    // ATTACHMENT (TEMP)
    // =========================
    [ObservableProperty]
    private string? pendingFilePath;

    public bool HasPendingFile => !string.IsNullOrWhiteSpace(PendingFilePath);

    partial void OnPendingFilePathChanged(string? value)
    {
        OnPropertyChanged(nameof(HasPendingFile));
    }

    // =========================
    // COMMANDS
    // =========================

    [RelayCommand]
    private async Task UploadDocumentAsync()
    {
        var dialog = new OpenFileDialog
        {
            AllowMultiple = false,
            Title = "Dokument auswählen",
            Filters =
            {
                new FileDialogFilter
                {
                    Name = "Dokumente",
                    Extensions = { "pdf", "txt", "docx" }
                }
            }
        };

        if (Avalonia.Application.Current?.ApplicationLifetime
            is not IClassicDesktopStyleApplicationLifetime desktop)
            return;

        var parent = desktop.MainWindow;
        if (parent is null)
            return;

        var result = await dialog.ShowAsync(parent);

        if (result is { Length: > 0 })
        {
            PendingFilePath = result[0];
        }
    }

    [RelayCommand]
    private void ClearAttachment()
    {
        PendingFilePath = null;
    }

    [RelayCommand]
    private async Task SendAsync()
    {
        if (IsSending)
            return;

        if (string.IsNullOrWhiteSpace(InputText) && !HasPendingFile)
            return;

        var userText = InputText.Trim();
        var filePath = PendingFilePath;

        InputText = string.Empty;
        PendingFilePath = null;

        var displayText = userText;

        if (!string.IsNullOrWhiteSpace(filePath))
        {
            var fileName = System.IO.Path.GetFileName(filePath);
            displayText = string.IsNullOrWhiteSpace(userText)
                ? $"📎 {fileName}"
                : $"{userText}\n📎 {fileName}";
        }

        Messages.Add(new ChatMessage
        {
            Sender = "User",
            FullText = displayText,
            DisplayedText = displayText
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
                Text = "Fehler: Kein Benutzer angemeldet!"
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
                    Text = "Fehler bei der KI-Generierung"
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
                    Text = "Fehler beim Generieren des Lernplans"
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
                Text = $"✅ Lernplan erfolgreich erstellt!\n📚 Thema: {studyPlan.Topic}\n📅 Tage: {studyPlan.Days.Count}\n\nDu kannst den Plan jetzt in der Lernplan-View sehen."
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Fehler beim Erstellen des Lernplans: {ex.Message}\n{ex.StackTrace}");
            Messages.Add(new ChatMessage
            {
                Sender = "System",
                Text = $"Fehler: {ex.Message}"
            });
        }
        finally
        {
            IsCreatingPlan = false;
        }
    }
}
