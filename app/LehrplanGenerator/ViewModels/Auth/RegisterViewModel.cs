using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.Logic.Utils;
using LehrplanGenerator.ViewModels.Shell;

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

    public RegisterViewModel(AuthService authService, INavigationService navigationService, AppState appState)
    {
        _authService = authService;
        _navigationService = navigationService;
        _appState = appState;
    }

    [RelayCommand]
    private async Task Register()
    {
        ErrorMessage = ValidateInput();
        if (ErrorMessage != null)
            return;

        try
        {
            var success = await _authService.RegisterAsync(FirstName, LastName, Email, Password);
            if (!success)
            {
                ErrorMessage = "Benutzer mit dieser E-Mail existiert bereits";
                return;
            }

            AppState.CurrentUser = _authService.CurrentUser;
            _navigationService.NavigateTo<ShellViewModel>();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Registrierungsfehler: {ex.Message}";
        }
    }

    [RelayCommand]
    private void Menu()
    {
        _navigationService.NavigateTo<LehrplanGenerator.ViewModels.Main.MainViewModel>();
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
}
