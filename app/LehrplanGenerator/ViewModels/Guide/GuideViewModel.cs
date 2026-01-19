using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.ViewModels.Shell;

namespace LehrplanGenerator.ViewModels.Guide;

public partial class GuideViewModel : ViewModelBase
{
    private readonly AppState _appState;
    private readonly INavigationService _navigationService;

    // =====================
    // DATA
    // =====================

    public IReadOnlyList<string> StudyPrograms { get; } =
        new[] { "AIN", "ITP" };

    public IReadOnlyList<int> Semesters { get; } =
        new[] { 1, 2, 3, 4, 5, 6, 7 };

    // =====================
    // STATE
    // =====================

    [ObservableProperty]
    private string? selectedStudyProgram;

    [ObservableProperty]
    private int? selectedSemester;

    // OPTIONAL
    [ObservableProperty]
    private string? schedulePdfPath;

    [ObservableProperty]
    private string? errorMessage;

    // 🔥 NEU: Wurde bereits versucht zu submitten?
    [ObservableProperty]
    private bool triedSubmit;

    // =====================
    // VALIDATION FLAGS (für View)
    // =====================

    public bool ShowStudyProgramError =>
        TriedSubmit && string.IsNullOrWhiteSpace(SelectedStudyProgram);

    public bool ShowSemesterError =>
        TriedSubmit && SelectedSemester is null;

    public bool HasGlobalError =>
        TriedSubmit && !string.IsNullOrWhiteSpace(ErrorMessage);

    // =====================
    // CTOR
    // =====================

    public GuideViewModel(AppState appState, INavigationService navigationService)
    {
        _appState = appState;
        _navigationService = navigationService;
    }

    // =====================
    // COMMAND
    // =====================

    [RelayCommand]
    private void Generate()
    {
        TriedSubmit = true;

        ErrorMessage = ValidateInput();
        NotifyValidationChanged();

        if (ErrorMessage != null)
            return;

        // TODO: später sauber ins AppState schreiben
        // _appState.StudyProgram = SelectedStudyProgram!;
        // _appState.Semester = SelectedSemester!.Value;
        // _appState.SchedulePdfPath = SchedulePdfPath;

        _navigationService.NavigateTo<ShellViewModel>();
    }

    // =====================
    // VALIDATION
    // =====================

    private string? ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(SelectedStudyProgram))
            return "Bitte wähle deinen Studiengang aus.";

        if (SelectedSemester is null)
            return "Bitte wähle dein aktuelles Semester aus.";

        return null;
    }

    // =====================
    // CHANGE TRACKING
    // =====================

    partial void OnSelectedStudyProgramChanged(string? value)
        => NotifyValidationChanged();

    partial void OnSelectedSemesterChanged(int? value)
        => NotifyValidationChanged();

    private void NotifyValidationChanged()
    {
        OnPropertyChanged(nameof(ShowStudyProgramError));
        OnPropertyChanged(nameof(ShowSemesterError));
        OnPropertyChanged(nameof(HasGlobalError));
    }
}
