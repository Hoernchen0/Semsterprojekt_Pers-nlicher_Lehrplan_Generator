using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.ViewModels.Auth;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.ViewModels.Guide;
using LehrplanGenerator.ViewModels.Shell;

namespace LehrplanGenerator.ViewModels.Main;

public class MainViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public MainViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        LoginCommand = new RelayCommand(NavigateToLogin);
        RegisterCommand = new RelayCommand(NavigateToRegister);
    }

    public IRelayCommand LoginCommand { get; }
    public IRelayCommand RegisterCommand { get; }

    private void NavigateToLogin() => _navigationService.NavigateTo<LoginViewModel>();
    private void NavigateToRegister() => _navigationService.NavigateTo<RegisterViewModel>();
}