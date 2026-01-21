using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.Logic.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LehrplanGenerator.ViewModels.StudyPlan;

public enum StudyPlanViewMode
{
    List,
    Calendar
}

public partial class StudyPlanViewModel : ViewModelBase
{
    private readonly AppState _appState;
    private readonly LearningProgressService _learningProgressService;

    [ObservableProperty]
    private ObservableCollection<StudyPlanEntity> studyPlans = new();

    [ObservableProperty]
    private StudyPlanEntity? selectedStudyPlan;

    [ObservableProperty]
    private ObservableCollection<DayPlanViewModel> days = new();

    [ObservableProperty]
    private DayPlanViewModel? selectedDay;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? errorMessage;

    // =================================================
    // 🔥 NEU: VIEW MODE
    // =================================================

    [ObservableProperty]
    private StudyPlanViewMode viewMode = StudyPlanViewMode.List;

    public bool IsListView => ViewMode == StudyPlanViewMode.List;
    public bool IsCalendarView => ViewMode == StudyPlanViewMode.Calendar;

    public string CurrentViewModeLabel =>
        ViewMode == StudyPlanViewMode.List ? "Kalender" : "Liste";

    // =================================================

    public StudyPlanViewModel(
        AppState appState,
        LearningProgressService learningProgressService)
    {
        _appState = appState;
        _learningProgressService = learningProgressService;
        _ = LoadPlansAsync();
    }

    private async Task LoadPlansAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        if (_appState.CurrentUserId == null)
            return;

        StudyPlans.Clear();

        var plans = await _learningProgressService
            .LoadStudyPlansAsync(_appState.CurrentUserId.Value);

        foreach (var p in plans)
            StudyPlans.Add(p);

        SelectedStudyPlan =
            plans.FirstOrDefault(p => p.Id == _appState.CurrentStudyPlanId)
            ?? plans.FirstOrDefault();

        IsLoading = false;
    }

    partial void OnSelectedStudyPlanChanged(StudyPlanEntity? value)
    {
        if (value == null)
            return;

        _appState.CurrentStudyPlanId = value.Id;
        _ = LoadPlanAsync(value.Id);
    }

    private async Task LoadPlanAsync(Guid planId)
    {
        Days.Clear();

        var result = await _learningProgressService.LoadStudyPlanAsync(planId);

        foreach (var day in result.GroupedDays)
        {
            Days.Add(new DayPlanViewModel(
            DateTime.Parse(day.Key).ToString("dd.MM.yyyy"),
            day.Value.Select(lp => new TaskItemViewModel(lp))
            ));
        }

        SelectedDay = Days.FirstOrDefault();
    }

    // =================================================
    // COMMANDS
    // =================================================

    [RelayCommand]
    private void SelectDay(DayPlanViewModel day)
    {
        SelectedDay = day;
    }

    [RelayCommand]
    private void ToggleViewMode()
    {
        ViewMode = ViewMode == StudyPlanViewMode.List
            ? StudyPlanViewMode.Calendar
            : StudyPlanViewMode.List;

        OnPropertyChanged(nameof(IsListView));
        OnPropertyChanged(nameof(IsCalendarView));
        OnPropertyChanged(nameof(CurrentViewModeLabel));
    }
}
