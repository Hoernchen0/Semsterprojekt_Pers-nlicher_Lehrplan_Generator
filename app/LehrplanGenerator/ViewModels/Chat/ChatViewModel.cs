using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.ViewModels.Main;
using LehrplanGenerator.Models.Chat;
using LehrplanGenerator.Logic.AI;
using LehrplanGenerator.Data;
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
    private readonly LearningProgressService _learningProgressService;
    private readonly ChatServiceDb _chatServiceDb;
    private Guid? _lastUserId;

    public ChatViewModel(
        INavigationService navigationService, 
        AppState appState,
        StudyPlanGeneratorService studyPlanGeneratorService,
        LearningProgressService learningProgressService,
        ChatServiceDb chatServiceDb)
    {
        _navigationService = navigationService;
        _appState = appState;
        _learningProgressService = learningProgressService;
        _chatServiceDb = chatServiceDb;
        _lastUserId = _appState.CurrentUserId;

        Messages.CollectionChanged += OnMessagesChanged;
        _appState.PropertyChanged += OnAppStatePropertyChanged;
        InitializeAsync();
    }

    private void OnAppStatePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppState.CurrentUserId) && _appState.CurrentUserId != _lastUserId)
        {
            _lastUserId = _appState.CurrentUserId;
            // Reload chat when user changes
            if (_appState.CurrentUserId.HasValue)
            {
                _ = ReloadChatAsync();
            }
        }
    }

    private async void InitializeAsync()
    {
        if (_appState.CurrentUserId.HasValue)
        {
            await LoadUserChatAsync();
        }
    }

    public async Task ReloadChatAsync()
    {
        Messages.Clear();
        _appState.CurrentSessionId = null;
        if (_appState.CurrentUserId.HasValue)
        {
            await LoadUserChatAsync();
        }
    }

    private async Task LoadUserChatAsync()
    {
        if (_appState.CurrentUserId == null) return;
        var sessions = (await _chatServiceDb.GetUserSessionsAsync(_appState.CurrentUserId.Value))
            .OrderByDescending(s => s.UpdatedAt)
            .ToList();
        if (sessions.Count == 0) return;
        var lastSession = sessions.First();
        _appState.CurrentSessionId = lastSession.SessionId;
        
        // Initialisiere die AI-Conversation mit der Session
        await _appState.AiService.InitializeConversationAsync(_appState.CurrentUserId.Value, lastSession.SessionId);
        
        var messages = await _chatServiceDb.GetSessionMessagesAsync(lastSession.SessionId, _appState.CurrentUserId.Value);
        Messages.Clear();
        foreach (var msg in messages)
        {
            // Filtere PDF_Context Messages aus der UI heraus (sind nur für AI-Kontext)
            if (msg.Sender == "PDF_Context")
                continue;
                
            Messages.Add(new ChatMessage { Sender = msg.Sender, FullText = msg.Text, DisplayedText = msg.Text });
        }
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

        // Ensure we have a session - create one if needed
        if (_appState.CurrentSessionId == null && _appState.CurrentUserId.HasValue)
        {
            var session = await _chatServiceDb.CreateSessionAsync(
                _appState.CurrentUserId.Value,
                "Chat Session",
                "");
            _appState.CurrentSessionId = session.SessionId;
            
            // Initialisiere die AI-Conversation mit der neuen Session
            await _appState.AiService.InitializeConversationAsync(_appState.CurrentUserId.Value, session.SessionId);
        }

        var userText = InputText.Trim();
        InputText = string.Empty;

        var userMessage = new ChatMessage
        {
            Sender = "User",
            FullText = userText,
            DisplayedText = userText
        };

        Messages.Add(userMessage);

        // Save user message to database
        if (_appState.CurrentUserId.HasValue && _appState.CurrentSessionId.HasValue)
        {
            await _chatServiceDb.SaveMessageAsync(
                _appState.CurrentSessionId.Value,
                _appState.CurrentUserId.Value,
                "User",
                userText);
        }

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
                
                // Save AI message to database
                if (_appState.CurrentUserId.HasValue && _appState.CurrentSessionId.HasValue)
                {
                    await _chatServiceDb.SaveMessageAsync(
                        _appState.CurrentSessionId.Value,
                        _appState.CurrentUserId.Value,
                        "AI",
                        response);
                }

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
            var studyPlan = await _appState.AiService.CreateStudyPlanAsync();

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

            await _learningProgressService.SaveStudyPlanAsync(_appState.CurrentUserId.Value, studyPlan);

            Messages.Add(new ChatMessage
            {
                Sender = "System",
                FullText = $"✓ Lernplan erstellt: {studyPlan.Topic} ({studyPlan.Days.Count} Tage)",
                DisplayedText = $"✓ Lernplan erstellt: {studyPlan.Topic} ({studyPlan.Days.Count} Tage)"
            });
        }
        catch (Exception ex)
        {
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