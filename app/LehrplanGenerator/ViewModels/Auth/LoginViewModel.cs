using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.ViewModels.Main;

namespace LehrplanGenerator.ViewModels.Auth;

public partial class LoginViewModel : ViewModelBase
{
    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string result = string.Empty;

    private readonly INavigationService _navigationService;
    private readonly AppState _appState;

    public LoginViewModel(INavigationService navigationService, AppState appState)
    {
        _navigationService = navigationService;
        _appState = appState;
    }

    [RelayCommand]
    private void Login()
    {
        _navigationService.NavigateTo<MainViewModel>();
    }

    [RelayCommand]
    private void Cancel()
    {
        _navigationService.NavigateTo<Main.MainViewModel>();
    }
}
