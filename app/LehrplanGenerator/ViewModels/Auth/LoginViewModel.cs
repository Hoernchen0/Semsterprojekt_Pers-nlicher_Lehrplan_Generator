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

        AppState.CurrentUser = new LehrplanGenerator.Logic.Models.User(cred.UserId, cred.FirstName, cred.LastName);

        Result = "Login erfolgreich";

        _navigationService.NavigateTo<ShellViewModel>();
    }

    [RelayCommand]
    private void Cancel()
    {
        _navigationService.NavigateTo<Main.MainViewModel>();
    }
}
