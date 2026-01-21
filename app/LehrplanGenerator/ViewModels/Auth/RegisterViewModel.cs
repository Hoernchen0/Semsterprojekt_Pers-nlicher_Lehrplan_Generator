using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.Logic.Utils;
using LehrplanGenerator.ViewModels.Guide;
using LehrplanGenerator.ViewModels.Main;
using LehrplanGenerator.ViewModels.Shell;

namespace LehrplanGenerator.ViewModels.Auth;

public partial class RegisterViewModel : ViewModelBase
{
    [ObservableProperty] private string firstName = string.Empty;
    [ObservableProperty] private string lastName = string.Empty;
    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string? errorMessage;

    private readonly UserCredentialStore _store;
    private readonly INavigationService _navigationService;
    private readonly AppState _appState;

    public RegisterViewModel(
        UserCredentialStore store,
        INavigationService navigationService,
        AppState appState)
    {
        _store = store;
        _navigationService = navigationService;
        _appState = appState;
    }

    [RelayCommand]
    private void Register()
    {
        try
        {

            ErrorMessage = ValidateInput();
            if (ErrorMessage != null)
            {
                return;
            }

            var userId = Guid.NewGuid();

            var credential = new UserCredential(
                userId,
                FirstName,
                LastName,
                Username,
                PasswordHasher.Hash(Password)
            );

            _store.Add(credential);

            _appState.CurrentUserId = credential.UserId;
            _appState.CurrentUserDisplayName = $"{credential.FirstName} {credential.LastName}";

            _navigationService.NavigateTo<GuideViewModel>();
        }
        catch (Exception ex)
        {

            ErrorMessage = $"Fehler bei der Registrierung: {ex.Message}";
        }

    }

    [RelayCommand]
    private void Menu()
    {
        _navigationService.NavigateTo<MainViewModel>();
    }

    private string? ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(FirstName) || FirstName.Length < 2)
            return "Vorname muss mindestens 2 Zeichen lang sein.";

        if (string.IsNullOrWhiteSpace(LastName) || LastName.Length < 2)
            return "Nachname muss mindestens 2 Zeichen lang sein.";

        if (string.IsNullOrWhiteSpace(Username) ||
            !Username.All(c => char.IsLetterOrDigit(c) || c == '_'))
            return "Username darf nur Buchstaben, Zahlen und Unterstriche enthalten.";

        if (_store.UsernameExists(Username))
            return "Username ist bereits vergeben.";

        if (string.IsNullOrWhiteSpace(Password) || Password.Length < 8
            || !Password.Any(char.IsUpper)
            || !Password.Any(char.IsLower)
            || !Password.Any(char.IsDigit)
            || !Password.Any(c => !char.IsLetterOrDigit(c)))
            return "Passwort: mindestens 8 Zeichen, Groß-/Kleinbuchstabe, Zahl und Sonderzeichen.";

        return null;
    }

    [RelayCommand]
    private void NavigateToLogin()
    {
        _navigationService.NavigateTo<LoginViewModel>();
    }
}
