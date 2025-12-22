using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.ViewModels.Main;
using System.Collections.ObjectModel;

namespace LehrplanGenerator.ViewModels.Chat;

public partial class ChatViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly AppState _appState;

    public ChatViewModel(INavigationService navigationService, AppState appState)
    {
        _navigationService = navigationService;
        _appState = appState;
        Messages = new ObservableCollection<ChatMessage>();
    }

    [ObservableProperty]
    private ObservableCollection<ChatMessage> _messages;

    [ObservableProperty]
    private string _inputText = string.Empty;

    [RelayCommand]
    private void Send()
    {
        if (string.IsNullOrWhiteSpace(InputText))
            return;

        Messages.Add(new ChatMessage
        {
            Sender = AppState.CurrentUserDisplayName ?? "Benutzer",
            Text = InputText
        });

        InputText = string.Empty;
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
