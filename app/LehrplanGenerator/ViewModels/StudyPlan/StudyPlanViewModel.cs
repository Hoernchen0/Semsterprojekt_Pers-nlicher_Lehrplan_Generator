using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.Logic.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LehrplanGenerator.Logic.AI;

namespace LehrplanGenerator.ViewModels.StudyPlan;

public enum StudyPlanViewMode
{
    List,
    Calendar,
    DayDetail
}

public partial class StudyPlanViewModel : ViewModelBase
{
    private readonly AppState _appState;
    private readonly LearningProgressService _learningProgressService;
    private readonly StudyPlanGeneratorService _studyPlanGeneratorService;
    private StudyPlanViewMode _lastViewMode;

    // =================================================
    // DATA
    // =================================================

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

    [ObservableProperty]
    private bool isCreatingPlan;

    // =================================================
    // VIEW MODE
    // =================================================

    [ObservableProperty]
    private StudyPlanViewMode viewMode = StudyPlanViewMode.List;

    public bool IsListView => ViewMode == StudyPlanViewMode.List;
    public bool IsCalendarView => ViewMode == StudyPlanViewMode.Calendar;
    public bool IsDayDetailView => ViewMode == StudyPlanViewMode.DayDetail;

    public string CurrentViewModeLabel =>
        ViewMode switch
        {
            StudyPlanViewMode.List => "Kalender",
            StudyPlanViewMode.Calendar => "Liste",
            StudyPlanViewMode.DayDetail => "Zurück",
            _ => ""
        };

    // =================================================
    // CALENDAR
    // =================================================

    [ObservableProperty]
    private DayPlanViewModel? selectedCalendarDay;

    public CalendarViewModel Calendar { get; }

    // =================================================
    // CTOR
    // =================================================

    public StudyPlanViewModel(
        AppState appState,
        LearningProgressService learningProgressService,
        StudyPlanGeneratorService studyPlanGeneratorService)
    {
        _appState = appState;
        _learningProgressService = learningProgressService;
        _studyPlanGeneratorService = studyPlanGeneratorService;

        Calendar = new CalendarViewModel(OpenDayView, this);
        _ = LoadPlansAsync();
    }

    // =================================================
    // LOAD PLANS
    // =================================================

    private async Task LoadPlansAsync()
    {
        if (_appState.CurrentUserId == null)
            return;

        IsLoading = true;
        ErrorMessage = null;

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
        Calendar.GenerateMonth();
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
        ViewMode = ViewMode switch
        {
            StudyPlanViewMode.List => StudyPlanViewMode.Calendar,
            StudyPlanViewMode.Calendar => StudyPlanViewMode.List,
            StudyPlanViewMode.DayDetail => StudyPlanViewMode.Calendar,
            _ => StudyPlanViewMode.List
        };

        SelectedCalendarDay = null;

        NotifyViewModeChanged();
    }

    // =================================================
    // CALENDAR / DAY DETAIL
    // =================================================

    [RelayCommand]
    private void OpenDayView(DayPlanViewModel day)
    {
        _lastViewMode = ViewMode;
        SelectedCalendarDay = day;
        ViewMode = StudyPlanViewMode.DayDetail;
        NotifyViewModeChanged();
    }

    [RelayCommand]
    private void BackFromDayDetail()
    {
        SelectedCalendarDay = null;

        ViewMode = _lastViewMode switch
        {
            StudyPlanViewMode.List => StudyPlanViewMode.List,
            StudyPlanViewMode.Calendar => StudyPlanViewMode.Calendar,
            _ => StudyPlanViewMode.List
        };

        NotifyViewModeChanged();
    }


    private void NotifyViewModeChanged()
    {
        OnPropertyChanged(nameof(IsListView));
        OnPropertyChanged(nameof(IsCalendarView));
        OnPropertyChanged(nameof(IsDayDetailView));
        OnPropertyChanged(nameof(CurrentViewModeLabel));
    }

    // =================================================
    // CREATE STUDY PLAN
    // =================================================

    [RelayCommand]
    private async Task CreateNewPlan()
    {
        if (IsCreatingPlan || _appState.CurrentUserId == null)
            return;

        IsCreatingPlan = true;
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var studyPlan = await _appState.AiService.CreateStudyPlanAsync();

            if (studyPlan == null)
            {
                ErrorMessage = "Fehler beim Generieren des Lernplans";
                return;
            }

            await _learningProgressService
                .SaveStudyPlanAsync(_appState.CurrentUserId.Value, studyPlan);

            await LoadPlansAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsCreatingPlan = false;
            IsLoading = false;
        }
    }

    // =================================================
    // DELETE STUDY PLAN
    // =================================================

    [RelayCommand]
    private async Task DeleteStudyPlan()
    {
        if (SelectedStudyPlan == null)
            return;

        IsLoading = true;
        ErrorMessage = null;

        try
        {
            await _learningProgressService
                .DeleteStudyPlanAsync(SelectedStudyPlan.Id);

            Days.Clear();
            SelectedDay = null;
            SelectedCalendarDay = null;
            SelectedStudyPlan = null;

            ViewMode = StudyPlanViewMode.List;
            NotifyViewModeChanged();

            await LoadPlansAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
