using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.ViewModels.Main;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace LehrplanGenerator.ViewModels.Chat;

public partial class ChatViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly AppState _appState;
    private readonly StudyPlanGeneratorService _aiService;

    public ChatViewModel(INavigationService navigationService, AppState appState)
    {
        _navigationService = navigationService;
        _appState = appState;
        _aiService = new StudyPlanGeneratorService();
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
            Sender = _appState.CurrentUserDisplayName ?? "Benutzer",
            Text = userMessage
        });

        IsSending = true;

        try
        {
            // API aufrufen
            var response = await _aiService.AskGptAsync(userMessage);

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
        catch (System.Exception ex)
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
