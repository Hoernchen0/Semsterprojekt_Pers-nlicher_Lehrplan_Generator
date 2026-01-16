using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.Logic.Utils;
using LehrplanGenerator.ViewModels.Shell;
using LehrplanGenerator.ViewModels.Windows;
using LehrplanGenerator.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace LehrplanGenerator.ViewModels.Auth;

public partial class RegisterViewModel : ViewModelBase
{
    [ObservableProperty] private string firstName = string.Empty;
    [ObservableProperty] private string lastName = string.Empty;
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string? errorMessage;

    private readonly AuthService _authService;
    private readonly INavigationService _navigationService;
    private readonly AppState _appState;
    private readonly IServiceProvider _serviceProvider;

    public RegisterViewModel(AuthService authService, INavigationService navigationService, AppState appState, IServiceProvider serviceProvider)
    {
        _authService = authService;
        _navigationService = navigationService;
        _appState = appState;
        _serviceProvider = serviceProvider;
    }

    [RelayCommand]
    private async Task Register()
    {
        ErrorMessage = ValidateInput();
        if (ErrorMessage != null)
            return;

        try
        {
            ErrorMessage = "Registrierung läuft...";
            var success = await _authService.RegisterAsync(FirstName, LastName, Email, Password);
            if (!success)
            {
                ErrorMessage = "Benutzer mit dieser E-Mail existiert bereits";
                return;
            }

            AppState.CurrentUser = _authService.CurrentUser;
            ErrorMessage = null;
            
            // Nach erfolgreichem Registrieren: Wechsel vom Registrierungsfenster zum MainWindow
            await Task.Delay(500);
            SwitchToMainWindow();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Registrierungsfehler: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"RegisterViewModel.Register() Exception: {ex}");
        }
    }

    [RelayCommand]
    private void Menu()
    {
        // Zurück zur Login-Seite
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new Views.Windows.LoginWindow
            {
                DataContext = _serviceProvider.GetRequiredService<LoginViewModel>()
            };
        }
    }

    private string? ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(FirstName) || FirstName.Length < 2)
            return "Vorname muss mindestens 2 Zeichen lang sein.";

        if (string.IsNullOrWhiteSpace(LastName) || LastName.Length < 2)
            return "Nachname muss mindestens 2 Zeichen lang sein.";

        if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@"))
            return "Bitte geben Sie eine gültige E-Mail-Adresse ein.";

        if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
            return "Passwort muss mindestens 6 Zeichen lang sein.";

        return null;
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
            ErrorMessage = $"Fehler beim Laden der Hauptanwendung: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"SwitchToMainWindow Exception: {ex}");
        }
    }
}
