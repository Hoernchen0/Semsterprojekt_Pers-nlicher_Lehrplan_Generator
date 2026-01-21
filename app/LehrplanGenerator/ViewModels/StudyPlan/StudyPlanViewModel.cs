using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Logic.AI;
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
    private readonly StudyPlanGeneratorService _studyPlanGeneratorService;

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
        LearningProgressService learningProgressService,
        StudyPlanGeneratorService studyPlanGeneratorService)
    {
        _appState = appState;
        _learningProgressService = learningProgressService;
        _studyPlanGeneratorService = studyPlanGeneratorService;
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



    // =================================================
    // CREATE NEW STUDY PLAN
    // =================================================
    [RelayCommand]
    private async Task CreateNewPlan()
    {
        if (IsCreatingPlan)
            return;

        if (_appState.CurrentUserId == null)
        {
            ErrorMessage = "Fehler: Kein Benutzer angemeldet!";
            return;
        }

        IsCreatingPlan = true;
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            // Generiere den StudyPlan
            var studyPlan = await _appState.AiService.CreateStudyPlanAsync();

            if (studyPlan == null)
            {
                ErrorMessage = "Fehler beim Generieren des Lernplans";
                return;
            }

            // Speichere den Plan in der Datenbank
            await _learningProgressService.SaveStudyPlanAsync(_appState.CurrentUserId.Value, studyPlan);

            // Lade die Pläne neu, um den neuen Plan anzuzeigen
            await LoadPlansAsync();

            ErrorMessage = null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Erstellen des Lernplans: {ex.Message}\n{ex.StackTrace}");
            ErrorMessage = $"Fehler: {ex.Message}";
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
        {
            ErrorMessage = "Kein Lernplan ausgewählt!";
            return;
        }

        IsLoading = true;
        ErrorMessage = null;

        try
        {
            await _learningProgressService.DeleteStudyPlanAsync(SelectedStudyPlan.Id);
            
            // Leere die Tages-Ansicht
            Days.Clear();
            SelectedDay = null;
            
            // Lade die Plans neu
            await LoadPlansAsync();

            ErrorMessage = null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Löschen des Lernplans: {ex.Message}\n{ex.StackTrace}");
            ErrorMessage = $"Fehler: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
