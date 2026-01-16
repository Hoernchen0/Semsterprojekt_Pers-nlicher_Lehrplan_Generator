using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.ViewModels.Main;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using Avalonia.Platform.Storage;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using LehrplanGenerator.Models.Chat;
namespace LehrplanGenerator.ViewModels.Chat;

public partial class ChatViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly AppState _appState;

    public ChatViewModel(INavigationService navigationService, AppState appState)
    {
        _navigationService = navigationService;
        _appState = appState;
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

        // User-Nachricht hinzufügen
        Messages.Add(new ChatMessage
        {
            Sender = _appState.CurrentUserDisplayName ?? "Benutzer",
            Text = userMessage
        });

        CanCreateStudyPlan = true;
        IsSending = true;

        try
        {
            // API aufrufen
            var response = await _appState.AiService.AskGptAsync(userMessage);

            if (!string.IsNullOrEmpty(response))
            {
                // Assistant-Antwort hinzufügen
                Messages.Add(new ChatMessage
                {
                    Sender = "KI-Assistent",
                    Text = response
                });
            }
        }
        catch (Exception ex)
        {
            Messages.Add(new ChatMessage
            {
                Sender = "System",
                Text = $"Fehler bei der Kommunikation mit der KI: {ex.Message}"
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

                Messages.Add(new ChatMessage
                {
                    Sender = "System",
                    Text = $"✓ Lernplan wurde erfolgreich erstellt!\nThema: {studyPlan.Topic}\nTage: {studyPlan.Days.Count}"
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
