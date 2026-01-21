using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.ViewModels.Chat;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LehrplanGenerator.ViewModels.Shell;

namespace LehrplanGenerator.ViewModels.Dashboard;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly AppState _appState;
    private readonly LearningProgressService _learningProgressService;
    private readonly INavigationService _navigationService;

    // =====================
    // HEADER
    // =====================

    [ObservableProperty]
    private string welcomeMessage = string.Empty;

    [ObservableProperty]
    private string userName = string.Empty;

    // =====================
    // STUDY PLANS
    // =====================

    public ObservableCollection<StudyPlanEntity> ActiveStudyPlans { get; } = new();

    [ObservableProperty]
    private StudyPlanEntity? selectedStudyPlan;

    public bool HasNoStudyPlans => ActiveStudyPlans.Count == 0;

    // =====================
    // DASHBOARD DATA
    // =====================

    [ObservableProperty]
    private int progressPercent;

    [ObservableProperty]
    private string nextUnitTitle = "Kein Lernplan ausgewählt";

    [ObservableProperty]
    private string nextUnitTime = string.Empty;

    [ObservableProperty]
    private bool hasNextUnit;

    // =====================
    // COMMANDS
    // =====================

    [RelayCommand]
    private void CreateStudyPlan()
    {
        _appState.Shell?.ShowChatCommand.Execute(null);
    }


    [RelayCommand]
    private async Task ContinueLearningAsync()
    {
        if (_appState.CurrentStudyPlanId == null)
            return;

        var next = await _learningProgressService
            .GetNextUnitAsync(_appState.CurrentStudyPlanId.Value);

        if (next == null)
            return;

        _appState.CurrentStudySession = next;

        _navigationService.NavigateTo<StudySessionViewModel>(vm =>
        {
            vm.Init(next);
        });
    }

    // =====================
    // CONSTRUCTOR
    // =====================

    public DashboardViewModel(
        AppState appState,
        LearningProgressService learningProgressService,
        INavigationService navigationService)
    {
        _appState = appState;
        _learningProgressService = learningProgressService;
        _navigationService = navigationService;

        RefreshHeader();
        _ = LoadStudyPlansAsync();

        _appState.PropertyChanged += (_, __) => RefreshHeader();
    }

    private void RefreshHeader()
    {
        UserName = _appState.CurrentUserDisplayName ?? "Benutzer";
        WelcomeMessage = $"Willkommen zurück, {UserName}!";
    }

    // =====================
    // LOAD STUDY PLANS
    // =====================

    private async Task LoadStudyPlansAsync()
    {
        if (_appState.CurrentUserId == null)
            return;

        var plans = await _learningProgressService
            .LoadStudyPlansAsync(_appState.CurrentUserId.Value);

        ActiveStudyPlans.Clear();
        foreach (var plan in plans.Where(p => p.IsActive))
            ActiveStudyPlans.Add(plan);

        OnPropertyChanged(nameof(HasNoStudyPlans));

        SelectedStudyPlan =
            ActiveStudyPlans.FirstOrDefault(p => p.Id == _appState.CurrentStudyPlanId)
            ?? ActiveStudyPlans.FirstOrDefault();
    }

    // =====================
    // SELECTION
    // =====================

    partial void OnSelectedStudyPlanChanged(StudyPlanEntity? value)
    {
        if (value == null)
        {
            ResetDashboard();
            _appState.CurrentStudyPlanId = null;
            return;
        }

        _appState.CurrentStudyPlanId = value.Id;
        _ = LoadPlanDataAsync(value.Id);
    }

    private async Task LoadPlanDataAsync(Guid studyPlanId)
    {
        ProgressPercent =
            await _learningProgressService.GetProgressPercentAsync(studyPlanId);

        var next =
            await _learningProgressService.GetNextUnitAsync(studyPlanId);

        if (next == null)
        {
            HasNextUnit = false;
            NextUnitTitle = "Alles erledigt 🎉";
            NextUnitTime = string.Empty;
            return;
        }

        HasNextUnit = true;
        NextUnitTitle = $"{next.Subject} – {next.ModuleName}";
        NextUnitTime =
            $"{next.PlannedStart:dd.MM HH:mm} – {next.PlannedEnd:HH:mm}";
    }

    private void ResetDashboard()
    {
        ProgressPercent = 0;
        HasNextUnit = false;
        NextUnitTitle = "Kein Lernplan ausgewählt";
        NextUnitTime = string.Empty;
    }
}
