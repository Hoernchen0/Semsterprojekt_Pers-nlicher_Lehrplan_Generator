using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.ViewModels.Main;
using LehrplanGenerator.Data.Repositories;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using System.Linq;
using Avalonia.Platform.Storage;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using LehrplanGenerator.Models.Chat;
namespace LehrplanGenerator.ViewModels.Chat;

public partial class ChatViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly AppState _appState;
    private readonly PersistenceService _persistenceService;
    private readonly IChatRepository _chatRepository;

    public ChatViewModel(INavigationService navigationService, AppState appState, PersistenceService persistenceService, IChatRepository chatRepository)
    {
        _navigationService = navigationService;
        _appState = appState;
        _persistenceService = persistenceService;
        _chatRepository = chatRepository;
        
        // Lade alte Chat-Nachrichten beim Initialisieren
        LoadPreviousChatMessagesAsync();
    }
    
    private async void LoadPreviousChatMessagesAsync()
    {
        if (!_appState.CurrentUserId.HasValue)
            return;
            
        try
        {
            // Hole alle Chat-Sessions des Benutzers
            var sessions = await _chatRepository.GetUserSessionsAsync(_appState.CurrentUserId.Value);
            var sessionList = sessions.ToList();
            
            if (sessionList.Count == 0)
            {
                Console.WriteLine("Keine alten Chat-Sessions gefunden");
                return;
            }
            
            Console.WriteLine($"{sessionList.Count} Chat-Session(s) gefunden");
            
            // Lade Nachrichten der neuesten Session
            var latestSession = sessionList.First();
            _appState.CurrentChatSessionId = latestSession.SessionId;
            
            var messages = await _chatRepository.GetSessionMessagesAsync(latestSession.SessionId);
            var messageList = messages.ToList();
            
            if (messageList.Count == 0)
            {
                Console.WriteLine("ℹ Keine Nachrichten in dieser Session");
                return;
            }
            
            Console.WriteLine($"Lade {messageList.Count} Nachrichten...");
            
            // Füge alle Nachrichten in die UI ein
            Messages.Clear();
            foreach (var msg in messageList.OrderBy(m => m.CreatedAt))
            {
                Messages.Add(new ChatMessage
                {
                    Sender = msg.Sender,
                    Text = msg.Text
                });
                Console.WriteLine($"  • {msg.Sender}: {msg.Text.Substring(0, Math.Min(50, msg.Text.Length))}...");
            }
            
            Console.WriteLine($"{messageList.Count} Nachrichten wiederhergestellt");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Laden der Chat-Nachrichten: {ex}");
        }
    }

    public ObservableCollection<ChatMessage> Messages => _appState.ChatMessages;

    [ObservableProperty]
    private string _inputText = string.Empty;

    [ObservableProperty]
    private bool _isSending = false;

    [ObservableProperty]
    private bool _canCreateStudyPlan = false;

    [RelayCommand]
    private async Task SendAsync()
    {
        if (string.IsNullOrWhiteSpace(InputText) || IsSending)
            return;

        var userMessage = InputText;
        InputText = string.Empty;
        userMessage = userMessage.Trim();

        // Erstelle eine neue Chat-Session falls nicht vorhanden
        if (!_appState.CurrentChatSessionId.HasValue)
        {
            _appState.CurrentChatSessionId = Guid.NewGuid();
        }

        // User-Nachricht hinzufügen
        var displayMessage = new ChatMessage
        {
            Sender = _appState.CurrentUserDisplayName ?? "Benutzer",
            Text = userMessage
        };
        Messages.Add(displayMessage);

        // Speichere User-Message in der Datenbank
        if (_appState.CurrentUserId.HasValue)
        {
            try
            {
                Console.WriteLine($"Speichere User-Nachricht: '{userMessage}'");
                await _persistenceService.SaveChatMessageAsync(
                    _appState.CurrentUserId.Value,
                    _appState.CurrentChatSessionId.Value,
                    displayMessage.Sender,
                    userMessage
                );
                Console.WriteLine($"User-Nachricht gespeichert");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FEHLER beim Speichern der User-Nachricht: {ex}");
                Messages.Add(new ChatMessage
                {
                    Sender = "System",
                    Text = $"Nachricht konnte nicht gespeichert werden: {ex.Message}"
                });
            }
        }

        CanCreateStudyPlan = true;
        IsSending = true;

        try
        {
            // API aufrufen

            var response = await _appState.AiService.AskGptAsync(userMessage);

            if (response == null)
            {
                Console.WriteLine($"SendAsync: API hat NULL zurückgegeben!");
                Messages.Add(new ChatMessage
                {
                    Sender = "System",
                    Text = "Fehler: Die KI hat nicht geantwortet. Bitte überprüfe die Konsole für Details."
                });
            }
            else if (string.IsNullOrEmpty(response))
            {
                Console.WriteLine($"⚠ SendAsync: API hat leeren String zurückgegeben");
                Messages.Add(new ChatMessage
                {
                    Sender = "System",
                    Text = "⚠ Die KI hat eine leere Antwort gegeben."
                });
            }
            else
            {
                Console.WriteLine($"✓ SendAsync: Gültige Response erhalten");
                // Assistant-Antwort hinzufügen
                var aiMessage = new ChatMessage
                {
                    Sender = "KI-Assistent",
                    Text = response
                };
                Messages.Add(aiMessage);

                // Speichere KI-Antwort in der Datenbank
                if (_appState.CurrentUserId.HasValue)
                {
                    try
                    {
                        Console.WriteLine($"💾 Speichere KI-Antwort ({response.Length} Zeichen)");
                        await _persistenceService.SaveChatMessageAsync(
                            _appState.CurrentUserId.Value,
                            _appState.CurrentChatSessionId.Value,
                            "KI-Assistent",
                            response
                        );
                        Console.WriteLine($"✓ KI-Antwort gespeichert");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ FEHLER beim Speichern der KI-Antwort: {ex}");
                        Messages.Add(new ChatMessage
                        {
                            Sender = "System",
                            Text = $"⚠ KI-Antwort konnte nicht gespeichert werden: {ex.Message}"
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ SendAsync EXCEPTION: {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine($"   Stack: {ex.StackTrace}");
            Messages.Add(new ChatMessage
            {
                Sender = "System",
                Text = $"❌ Fehler bei der Kommunikation mit der KI: {ex.Message}"
            });
        }
        finally
        {
            IsSending = false;
        }
    }

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
                        Console.WriteLine($"📅 Speichere Lernplan mit {studyPlan.Days.Count} Tagen...");
                        foreach (var dayPlan in studyPlan.Days)
                        {
                            await _persistenceService.SaveCalendarAsync(
                                _appState.CurrentUserId.Value,
                                dayPlan.Day,
                                dayPlan
                            );
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

    [RelayCommand]
    private async Task UploadAsync()
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
                    Text = "Fehler: Hauptfenster nicht verfügbar."
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
                    Text = $"✓ PDF '{selectedFile.Name}' erfolgreich hochgeladen und verarbeitet!"
                });
            }
            else
            {
                Messages.Add(new ChatMessage
                {
                    Sender = "System",
                    Text = $"❌ Fehler beim Verarbeiten der PDF '{selectedFile.Name}'."
                });
            }
        }
        catch (Exception ex)
        {
            Messages.Add(new ChatMessage
            {
                Sender = "System",
                Text = $"Fehler beim Hochladen: {ex.Message}"
            });
        }
    }

    [RelayCommand]
    private void GoBack()
    {
        _navigationService.NavigateTo<MainViewModel>();
    }

    partial void OnInputTextChanged(string value) { OnPropertyChanged(nameof(HasInput)); }
    public bool HasInput => !string.IsNullOrWhiteSpace(InputText);
}
