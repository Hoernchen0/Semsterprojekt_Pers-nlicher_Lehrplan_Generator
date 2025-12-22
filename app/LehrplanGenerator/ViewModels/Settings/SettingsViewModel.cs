using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.ViewModels.Main;

namespace LehrplanGenerator.ViewModels.Settings;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly AppState _appState;

    public SettingsViewModel(INavigationService navigationService, AppState appState)
    {
        _navigationService = navigationService;
        _appState = appState;
    }

    [RelayCommand]
    private void LogOut()
    {
        AppState.CurrentUser = null;

        _navigationService.NavigateTo<MainViewModel>();
    }
}
