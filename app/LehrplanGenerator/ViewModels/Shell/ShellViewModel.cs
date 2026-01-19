using System;
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
using LehrplanGenerator.Data.Repositories;

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

    public ShellViewModel(UserCredentialStore store, INavigationService navigationService, AppState appState)
    {
        _store = store;
        _navigationService = navigationService;
        _appState = appState;

        if (!string.IsNullOrWhiteSpace(_appState.CurrentUserDisplayName))
        {
            CurrentUserName = _appState.CurrentUserDisplayName;
        }

        Console.WriteLine("=== ShellViewModel initialisiert ===");
        ShowHome();
    }

    [RelayCommand]
    private void ShowHome()
    {
        try
        {
            Console.WriteLine("ShowHome() Start");
            SelectedTab = "Home";
            
            Console.WriteLine("  → GetRequiredService<ICalendarRepository>");
            var calendarRepository = App.Services.GetRequiredService<ICalendarRepository>();
            Console.WriteLine($"  ✓ CalendarRepository erhalten");
            
            Console.WriteLine("  → Erstelle DashboardViewModel");
            CurrentContent = new DashboardViewModel(_appState, calendarRepository);
            Console.WriteLine("✓ Dashboard geladen");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ FEHLER beim Laden des Dashboards: {ex.GetType().Name}");
            Console.WriteLine($"❌ Nachricht: {ex.Message}");
            Console.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
            
            // Fallback
            try
            {
                Console.WriteLine("  → Fallback: Erstelle DashboardViewModel ohne Repository");
                SelectedTab = "Home";
                CurrentContent = new DashboardViewModel(_appState, null!);
                Console.WriteLine("✓ Dashboard (Fallback) geladen");
            }
            catch (Exception fallbackEx)
            {
                Console.WriteLine($"❌ FEHLER auch beim Fallback: {fallbackEx}");
            }
        }
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
        try
        {
            SelectedTab = "Chat";
            CurrentContent = App.Services.GetRequiredService<ChatViewModel>();
            Console.WriteLine("✓ Chat geladen");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Fehler beim Laden des Chat: {ex}");
        }
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
        _appState.CurrentUserId = null;
        _appState.CurrentUserDisplayName = null;

        _navigationService.NavigateTo<MainViewModel>();
    }
}
