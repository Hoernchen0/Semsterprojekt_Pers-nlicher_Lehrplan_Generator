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

        UserName = AppState.CurrentUserDisplayName ?? "Benutzer";
        WelcomeMessage = $"Willkommen zurück, {UserName}!";

        System.Diagnostics.Debug.WriteLine($"CurrentUserDisplayName: '{AppState.CurrentUserDisplayName}'");

        _appState.PropertyChanged += (_, __) =>
        {
            UserName = AppState.CurrentUserDisplayName ?? "Benutzer";
            WelcomeMessage = $"Willkommen zurück, {UserName}!";
        };
    }
}