using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.ViewModels.Main;
using LehrplanGenerator.ViewModels.Settings;
using LehrplanGenerator.ViewModels.Dashboard;
using LehrplanGenerator.ViewModels.Chat;
using LehrplanGenerator.ViewModels.StudyPlan;
using Microsoft.Extensions.DependencyInjection;

namespace LehrplanGenerator.ViewModels.Shell;

public partial class ShellViewModel : ViewModelBase
{
    private readonly UserCredentialStore _store;
    private readonly INavigationService _navigationService;
    private readonly AppState _appState;

    [ObservableProperty]
    private string currentUserName = string.Empty;

    [ObservableProperty]
    private ViewModelBase? currentContent;

    [ObservableProperty]
    private string selectedTab = "Home";

    public ShellViewModel(
        UserCredentialStore store,
        INavigationService navigationService,
        AppState appState)
    {
        _store = store;
        _navigationService = navigationService;
        _appState = appState;

        if (!string.IsNullOrWhiteSpace(_appState.CurrentUserDisplayName))
        {
            CurrentUserName = _appState.CurrentUserDisplayName;
        }

        ShowHome();
    }

    // =========================
    // HOME / DASHBOARD
    // =========================
    [RelayCommand]
    private void ShowHome()
    {
        SelectedTab = "Home";
        CurrentContent = App.Services.GetRequiredService<DashboardViewModel>();
    }

    // =========================
    // SETTINGS
    // =========================
    [RelayCommand]
    private void ShowSettings()
    {
        SelectedTab = "Settings";
        CurrentContent = App.Services.GetRequiredService<SettingsViewModel>();
    }

    // =========================
    // CHAT
    // =========================
    [RelayCommand]
    private void ShowChat()
    {
        SelectedTab = "Chat";
        CurrentContent = App.Services.GetRequiredService<ChatViewModel>();
    }

    // =========================
    // STUDY PLAN / KALENDER
    // =========================
    [RelayCommand]
    private void ShowStudyPlan()
    {
        SelectedTab = "Kalender";
        CurrentContent = App.Services.GetRequiredService<StudyPlanViewModel>();
    }

    // =========================
    // LOGOUT
    // =========================
    [RelayCommand]
    private void Logout()
    {
        _appState.CurrentUserId = null;
        _appState.CurrentUserDisplayName = null;
        _navigationService.NavigateTo<MainViewModel>();
    }
}
