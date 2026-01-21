using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.ViewModels.Shell;
using LehrplanGenerator.Logic.Utils;

namespace LehrplanGenerator.ViewModels.Auth;

public partial class LoginViewModel : ViewModelBase
{
    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string result = string.Empty;

    private readonly UserCredentialStore _store;
    private readonly INavigationService _navigationService;
    private readonly AppState _appState;

    public LoginViewModel(UserCredentialStore store, INavigationService navigationService, AppState appState)
    {
        _store = store;
        _navigationService = navigationService;
        _appState = appState;
    }

    [RelayCommand]
    private void Login()
    {
        try
        {
            var cred = _store.GetByUsername(Username);

            if (cred == null)
            {
                Result = "Benutzername oder Passwort falsch";
                return;
            }

            var hashed = PasswordHasher.Hash(Password);
            if (hashed != cred.PasswordHash)
            {
                Result = "Benutzername oder Passwort falsch";
                return;
            }

            _appState.CurrentUserId = cred.UserId;
            _appState.CurrentUserDisplayName = $"{cred.FirstName} {cred.LastName}";

            Result = "Login erfolgreich";
            Console.WriteLine($"✓ Login erfolgreich für {Username}");

            _navigationService.NavigateTo<ShellViewModel>();
        }
        catch (Exception ex)
        {
            Result = $"Fehler beim Login: {ex.Message}";
            Console.WriteLine($"❌ FEHLER beim Login: {ex}");
        }
    }

    [RelayCommand]
    private void NavigateToRegister()
    {
        _navigationService.NavigateTo<RegisterViewModel>();
    }
}
