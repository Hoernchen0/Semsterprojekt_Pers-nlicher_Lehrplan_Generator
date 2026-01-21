using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.Logic.Utils;
using LehrplanGenerator.Logic.Utils;
using LehrplanGenerator.ViewModels.Main;
using LehrplanGenerator.Views.Settings;

namespace LehrplanGenerator.ViewModels.Settings;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly AppState _appState;

    private readonly UserCredentialStore _store;

    public string CurrentUserDisplayName => _appState.CurrentUserDisplayName;
    public string CurrentUsername => _appState.CurrentUsername;


    public SettingsViewModel(
    INavigationService navigationService,
    AppState appState,
    UserCredentialStore store)
    {
        _navigationService = navigationService;
        _appState = appState;
        _store = store;

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

    [RelayCommand]
    private void EditProfile()
    {
        var vm = new UsernameModificationViewModel(
            _appState.CurrentUsername,
            _store,
            _appState,
            result =>
            {
                OnPropertyChanged(nameof(CurrentUserDisplayName));
                OnPropertyChanged(nameof(CurrentUsername));
            });

        var lifetime = Avalonia.Application.Current?.ApplicationLifetime
                       as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime;

        var mainWindow = lifetime?.MainWindow;

        var window = new UsernameModificationView
        {
            DataContext = vm
        };

        window.ShowDialog(mainWindow);
    }


    [RelayCommand]
    private void EditPassword()
    {
        var vm = new PasswordModificationViewModel(
            _appState.CurrentUsername,
            _store,
            _appState,
            _ => { });

        var lifetime = Avalonia.Application.Current?.ApplicationLifetime
            as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime;

        var window = new PasswordModificationView
        {
            DataContext = vm
        };

        window.ShowDialog(lifetime?.MainWindow);
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
