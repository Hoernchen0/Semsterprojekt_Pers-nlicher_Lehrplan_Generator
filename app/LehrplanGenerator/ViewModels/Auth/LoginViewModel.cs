using System;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.ViewModels.Shell;
using LehrplanGenerator.ViewModels.Windows;
using LehrplanGenerator.Views.Windows;
using LehrplanGenerator.Logic.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace LehrplanGenerator.ViewModels.Auth;

public partial class LoginViewModel : ViewModelBase
{
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string result = string.Empty;

    private readonly AuthService _authService;
    private readonly INavigationService _navigationService;
    private readonly AppState _appState;
    private readonly IServiceProvider _serviceProvider;

    public LoginViewModel(AuthService authService, INavigationService navigationService, AppState appState, IServiceProvider serviceProvider)
    {
        _authService = authService;
        _navigationService = navigationService;
        _appState = appState;
        _serviceProvider = serviceProvider;
    }

    [RelayCommand]
    private async Task Login()
    {
        try
        {
            Result = "Anmelden läuft...";
            System.Diagnostics.Debug.WriteLine($"[LoginViewModel] Login-Button geklickt: Email={Email}");
            var user = await _authService.LoginAsync(Email, Password);

            if (user == null)
            {
                Result = "Benutzername oder Passwort falsch";
                return;
            }

            AppState.CurrentUser = user;
            Result = "Login erfolgreich! Laden...";

            // Nach erfolgreichem Login: Wechsel vom LoginWindow zum MainWindow
            await Task.Delay(500); // Kurze Verzögerung für UX
            SwitchToMainWindow();
        }
        catch (Exception ex)
        {
            Result = $"Login-Fehler: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"LoginViewModel.Login() Exception: {ex}");
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        // Zur Registrierungsseite navigieren
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new RegisterWindow
            {
                DataContext = _serviceProvider.GetRequiredService<RegisterViewModel>()
            };
        }
    }

    private void SwitchToMainWindow()
    {
        try
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindowVM = _serviceProvider.GetRequiredService<MainWindowViewModel>();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainWindowVM
                };
                desktop.MainWindow.Show();
            }
        }
        catch (Exception ex)
        {
            Result = $"Fehler beim Laden der Hauptanwendung: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"SwitchToMainWindow Exception: {ex}");
        }
    }
}

