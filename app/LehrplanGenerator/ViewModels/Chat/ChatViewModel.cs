using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.ViewModels.Main;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using LehrplanGenerator.Logic.AI;
using System;

namespace LehrplanGenerator.ViewModels.Chat;

public partial class ChatViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly AppState _appState;

    public ChatViewModel(INavigationService navigationService, AppState appState)
    {
        _navigationService = navigationService;
        _appState = appState;
        _messages = new ObservableCollection<ChatMessage>();
    }

    [ObservableProperty]
    private ObservableCollection<ChatMessage> _messages;

    [ObservableProperty]
    private string _inputText = string.Empty;

    [ObservableProperty]
    private bool _isSending = false;

    [RelayCommand]
    private async Task SendAsync()
    {
        if (string.IsNullOrWhiteSpace(InputText) || IsSending)
            return;

        var userMessage = InputText;
        InputText = string.Empty;

        // User-Nachricht hinzufügen
        Messages.Add(new ChatMessage
        {
            Sender = AppState.CurrentUserDisplayName ?? "Benutzer",
            Text = userMessage
        });

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
    private void Upload()
    {
        //TODO
    }

    [RelayCommand]
    private void GoBack()
    {
        _navigationService.NavigateTo<MainViewModel>();
    }
}

public class ChatMessage
{
    public string Sender { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
