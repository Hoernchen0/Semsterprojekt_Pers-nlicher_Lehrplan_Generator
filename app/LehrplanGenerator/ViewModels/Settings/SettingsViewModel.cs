using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.Logic.Utils;
using LehrplanGenerator.ViewModels.Main;

namespace LehrplanGenerator.ViewModels.Settings;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly AppState _appState;

    public SettingsViewModel(
        INavigationService navigationService,
        AppState appState)
    {
        _navigationService = navigationService;
        _appState = appState;

        // Theme-Änderungen an UI weiterreichen
        ThemeManager.Instance.PropertyChanged += (_, __) =>
        {
            OnPropertyChanged(nameof(ThemeButtonText));
        };
    }

    // =====================
    // THEME
    // =====================

    public string ThemeButtonText =>
        ThemeManager.Instance.IsDark
            ? "Dark Mode aktiv"
            : "Light Mode aktiv";

    [RelayCommand]
    private void ToggleTheme()
    {
        ThemeManager.Instance.Toggle();
    }

    // =====================
    // ACCOUNT
    // =====================

    [RelayCommand]
    private void LogOut()
    {
        _appState.CurrentUserId = null;
        _appState.CurrentUserDisplayName = null;

        _navigationService.NavigateTo<MainViewModel>();
    }
}
