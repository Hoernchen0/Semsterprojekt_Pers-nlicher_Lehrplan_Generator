using CommunityToolkit.Mvvm.ComponentModel;
using LehrplanGenerator.Logic.State;

namespace LehrplanGenerator.ViewModels.Dashboard;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly AppState _appState;

    [ObservableProperty]
    private string welcomeMessage = string.Empty;

    [ObservableProperty]
    private string userName = string.Empty;

    public DashboardViewModel(AppState appState)
    {
        _appState = appState;

        UserName = _appState.CurrentUserDisplayName ?? "Benutzer";
        WelcomeMessage = $"Willkommen zurück, {UserName}!";

        System.Diagnostics.Debug.WriteLine($"CurrentUserDisplayName: '{_appState.CurrentUserDisplayName}'");

        _appState.PropertyChanged += (_, __) =>
        {
            UserName = _appState.CurrentUserDisplayName ?? "Benutzer";
            WelcomeMessage = $"Willkommen zurück, {UserName}!";
        };
    }
}