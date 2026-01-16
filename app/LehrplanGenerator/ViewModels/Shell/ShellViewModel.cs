using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.ViewModels.Main;
using LehrplanGenerator.ViewModels.Settings;
using LehrplanGenerator.ViewModels.Dashboard;
using Microsoft.Extensions.DependencyInjection;
using LehrplanGenerator.ViewModels.Chat;
using LehrplanGenerator.ViewModels.StudyPlan;

namespace LehrplanGenerator.ViewModels.Shell;

public partial class ShellViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly AppState _appState;

    [ObservableProperty]
    private string currentUserName = string.Empty;

    [ObservableProperty]
    private ViewModelBase? currentContent;

    [ObservableProperty]
    private string selectedTab = "Home";

    public ShellViewModel(INavigationService navigationService, AppState appState)
    {
        _navigationService = navigationService;
        _appState = appState;

        if (!string.IsNullOrWhiteSpace(AppState.CurrentUserDisplayName))
        {
            CurrentUserName = AppState.CurrentUserDisplayName!;
        }

        ShowHome();
    }

    [RelayCommand]
    private void ShowHome()
    {
        SelectedTab = "Home";
        CurrentContent = new DashboardViewModel(_appState);
    }

    [RelayCommand]
    private void ShowSettings()
    {
        SelectedTab = "Settings";
        CurrentContent = App.Services.GetRequiredService<SettingsViewModel>();
    }

    [RelayCommand]
    private void ShowChat()
    {
        SelectedTab = "Chat";
        CurrentContent = App.Services.GetRequiredService<ChatViewModel>();
    }

    [RelayCommand]
    private void ShowStudyPlan()
    {
        SelectedTab = "Kalender";
        CurrentContent = App.Services.GetRequiredService<StudyPlanViewModel>();
    }

    [RelayCommand]
    private void Logout()
    {
        AppState.CurrentUser = null;

        _navigationService.NavigateTo<MainViewModel>();
    }
}
