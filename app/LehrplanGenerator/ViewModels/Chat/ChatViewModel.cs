using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.ViewModels.Main;
using LehrplanGenerator.Models.Chat;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using System;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using LehrplanGenerator.Models.Chat;

namespace LehrplanGenerator.ViewModels.Chat;

public partial class ChatViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly AppState _appState;
    // private readonly PersistenceService persistenceService;

    public ChatViewModel(INavigationService navigationService, AppState appState)
    {
        _navigationService = navigationService;
        _appState = appState;

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
    // STUDY PLAN CREATION
    // =========================
    [ObservableProperty]
    private bool _canCreateStudyPlan = false;

    [RelayCommand]
    private async Task CreateStudyPlanAsync()
    {
        if (IsSending)
            return;

        IsSending = true;

        Messages.Add(new ChatMessage
        {
            Sender = "System",
            Text = "Erstelle Lernplan..."
        });

        try
        {
            // Erstelle Studienplan mit dem geteilten AI-Service
            var studyPlan = await _appState.AiService.CreateStudyPlanAsync();

            if (studyPlan != null)
            {
                // Speichere den Plan im AppState für die StudyPlanViewModel
                _appState.CurrentStudyPlan = studyPlan;

                // Speichere jeden Tag des Lernplans in der Datenbank
                if (_appState.CurrentUserId.HasValue)
                {
                    try
                    {
                        Console.WriteLine($"Speichere Lernplan mit {studyPlan.Days.Count} Tagen...");
                        foreach (var dayPlan in studyPlan.Days)
                        {
                          //  await _navigationService.SaveCalendarAsync(
                          //      _appState.CurrentUserId.Value,
                          //      dayPlan.Day,
                          //      dayPlan
                          //  );
                        }
                        Console.WriteLine($"✓ Lernplan vollständig gespeichert");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ FEHLER beim Speichern des Lernplans: {ex}");
                        Messages.Add(new ChatMessage
                        {
                            Sender = "System",
                            Text = $"⚠ Lernplan konnte nicht vollständig gespeichert werden: {ex.Message}"
                        });
                    }
                }

                Messages.Add(new ChatMessage
                {
                    Sender = "System",
                    Text = $"✓ Lernplan wurde erfolgreich erstellt und gespeichert!\nThema: {studyPlan.Topic}\nTage: {studyPlan.Days.Count}"
                });
            }
            else
            {
                Messages.Add(new ChatMessage
                {
                    Sender = "System",
                    Text = "Lernplan konnte nicht erstellt werden. Bitte geben Sie mehr Informationen im Chat an (Thema, Zeitraum, tägliche Lernzeit, etc.)"
                });
            }
        }
        catch (Exception ex)
        {
            Messages.Add(new ChatMessage
            {
                Sender = "System",
                Text = $"Fehler beim Erstellen des Lernplans: {ex.Message}"
            });
        }
        finally
        {
            IsSending = false;
        }
    }
}
