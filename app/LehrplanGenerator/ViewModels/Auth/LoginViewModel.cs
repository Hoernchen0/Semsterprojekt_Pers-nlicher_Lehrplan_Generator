using System;
using System.Threading.Tasks;
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

    private readonly AuthService _authService;
    private readonly INavigationService _navigationService;
    private readonly AppState _appState;

    public LoginViewModel(AuthService authService, INavigationService navigationService, AppState appState)
    {
        _authService = authService;
        _navigationService = navigationService;
        _appState = appState;
    }

    [RelayCommand]
    private async Task Login()
    {
        try
        {
            var user = await _authService.LoginAsync(Username, Password);

            if (user == null)
            {
                Result = "Benutzername oder Passwort falsch";
                return;
            }

            AppState.CurrentUser = user;
            Result = "Login erfolgreich";

            _navigationService.NavigateTo<ShellViewModel>();
        }
        catch (Exception ex)
        {
            Result = $"Login-Fehler: {ex.Message}";
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        _navigationService.NavigateTo<Main.MainViewModel>();
    }
}
