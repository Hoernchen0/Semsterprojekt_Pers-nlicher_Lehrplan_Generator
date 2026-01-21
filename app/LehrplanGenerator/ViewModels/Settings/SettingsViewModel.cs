using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
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
        var lifetime = Avalonia.Application.Current?.ApplicationLifetime
            as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime;

        var mainWindow = lifetime?.MainWindow;

        UsernameModificationView? window = null;

        var vm = new UsernameModificationViewModel(
            _appState.CurrentUsername,
            _store,
            _appState,
            result =>
            {
                if (result != null)
                {
                    OnPropertyChanged(nameof(CurrentUserDisplayName));
                    OnPropertyChanged(nameof(CurrentUsername));
                }

                window?.Close();
            });

        window = new UsernameModificationView
        {
            DataContext = vm
        };
        window.ShowDialog(mainWindow);
    }



    [RelayCommand]
    private void EditPassword()
    {
        var lifetime = Avalonia.Application.Current?.ApplicationLifetime
            as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime;

        var mainWindow = lifetime?.MainWindow;

        PasswordModificationView? window = null;

        var vm = new PasswordModificationViewModel(
            _appState.CurrentUsername,
            _store,
            _appState,
            _ => window?.Close()
        );

        window = new PasswordModificationView
        {
            DataContext = vm
        };

        window.ShowDialog(mainWindow);
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
