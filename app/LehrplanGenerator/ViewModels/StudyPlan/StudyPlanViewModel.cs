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
        if (_appState.CurrentUserId == null)
        {
            ErrorMessage = "Kein Benutzer angemeldet!";
            return;
        }

        IsLoading = true;
        ErrorMessage = null;

        try
        {
            Console.WriteLine($"📋 Starte KI-Lernplan-Generierung...");

            // Nutze den Chat-Kontext wenn vorhanden, sonst Standard-Prompt
            string prompt = "Erstelle einen Lernplan für Softwareentwicklung. " +
                "Plane die nächsten 5-7 Tage mit jeweils 3-4 Lerneinheiten à 50 Minuten mit 10 Minuten Pausen. " +
                "Das Studium beginnt morgen.";

            // Wenn es aktuelle Chat-Messages gibt, nutze diese als Kontext
            if (_appState.ChatMessages.Count > 0)
            {
                var lastUserMessage = _appState.ChatMessages
                    .LastOrDefault(m => m.Sender == "User");
                
                if (lastUserMessage != null)
                {
                    prompt = lastUserMessage.FullText;
                }
            }

            // Frage KI um einen Plan zu erstellen
            var planResponse = await _studyPlanGeneratorService.AskGptAsync(prompt);

            if (string.IsNullOrEmpty(planResponse))
            {
                ErrorMessage = "Fehler bei der KI-Generierung";
                IsLoading = false;
                return;
            }

            Console.WriteLine($"🤖 KI hat Lernplan geplant");

            // Generiere den StudyPlan
            var studyPlan = await _studyPlanGeneratorService.CreateStudyPlanAsync();

            if (studyPlan == null)
            {
                ErrorMessage = "Fehler beim Generieren des Lernplans";
                IsLoading = false;
                return;
            }

            Console.WriteLine($"📊 StudyPlan generiert mit {studyPlan.Days.Count} Tagen");

            // Speichere den Plan in der Datenbank
            await _learningProgressService.SaveStudyPlanAsync(_appState.CurrentUserId.Value, studyPlan);

            Console.WriteLine($"✅ Lernplan gespeichert: {studyPlan.Topic}");

            // Lade die Plans neu
            await LoadPlansAsync();

            ErrorMessage = null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Fehler beim Erstellen des Lernplans: {ex.Message}\n{ex.StackTrace}");
            ErrorMessage = $"Fehler: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
