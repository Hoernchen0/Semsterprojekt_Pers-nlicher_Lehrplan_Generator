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
    private readonly LearningProgressService _learningProgressService;

    public ChatViewModel(
        INavigationService navigationService,
        AppState appState,
        LearningProgressService learningProgressService)
    {
        _navigationService = navigationService;
        _appState = appState;
        _learningProgressService = learningProgressService;
    }

    public ObservableCollection<ChatMessage> Messages => _appState.ChatMessages;

    [ObservableProperty]
    private string _inputText = string.Empty;

    [ObservableProperty]
    private bool _isSending;

    [ObservableProperty]
    private bool _canCreateStudyPlan;

    [RelayCommand]
    private async Task SendAsync()
    {
        if (string.IsNullOrWhiteSpace(InputText) || IsSending)
            return;

        var userMessage = InputText.Trim();
        InputText = string.Empty;

        Messages.Add(new ChatMessage
        {
            Sender = _appState.CurrentUserDisplayName ?? "Benutzer",
            Text = userMessage
        });

        CanCreateStudyPlan = true;
        IsSending = true;

        try
        {
            var response = await _appState.AiService.AskGptAsync(userMessage);

            if (!string.IsNullOrEmpty(response))
            {
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
            var studyPlan = await _appState.AiService.CreateStudyPlanAsync();

            if (studyPlan == null)
            {
                Messages.Add(new ChatMessage
                {
                    Sender = "System",
                    Text = "Lernplan konnte nicht erstellt werden."
                });
                return;
            }

            // ✅ KORREKT: nur 2 Parameter
            await _learningProgressService.SaveStudyPlanAsync(
                _appState.CurrentUserId!.Value,
                studyPlan
            );

            Messages.Add(new ChatMessage
            {
                Sender = "System",
                Text = $"✓ Lernplan gespeichert\nThema: {studyPlan.Topic}\nTage: {studyPlan.Days.Count}"
            });
        }
        catch (Exception ex)
        {
            Messages.Add(new ChatMessage
            {
                Sender = "System",
                Text = $"Fehler beim Erstellen oder Speichern des Lernplans: {ex.Message}"
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
            var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var mainWindow = lifetime?.MainWindow;

            if (mainWindow == null)
                return;

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

            var files = await mainWindow.StorageProvider.OpenFilePickerAsync(options);
            if (files.Count == 0)
                return;

            var file = files[0];
            var success = await _appState.AiService.UploadPdfAsync(file.Path.LocalPath);

            Messages.Add(new ChatMessage
            {
                Sender = "System",
                Text = success
                    ? $"✓ PDF '{file.Name}' verarbeitet"
                    : $"❌ Fehler beim Verarbeiten von '{file.Name}'"
            });
        }
        catch (Exception ex)
        {
            Messages.Add(new ChatMessage
            {
                Sender = "System",
                Text = $"Upload-Fehler: {ex.Message}"
            });
        }
    }

    [RelayCommand]
    private void GoBack()
    {
        _navigationService.NavigateTo<MainViewModel>();
    }

    partial void OnInputTextChanged(string value)
        => OnPropertyChanged(nameof(HasInput));

    public bool HasInput => !string.IsNullOrWhiteSpace(InputText);
}
