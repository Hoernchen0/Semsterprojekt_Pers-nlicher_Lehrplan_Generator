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

    [ObservableProperty]
    private int currentStep = 1;

    public int totalSteps => 3;

    public IReadOnlyList<string> StudyPrograms { get; } =
        new[] { "AIN", "ITP" };

    [ObservableProperty]
    private string? selectedStudyProgram;

    public IReadOnlyList<int> Semesters { get; } =
        new[] { 1, 2, 3, 4, 5, 6, 7 };

    [ObservableProperty]
    private int? selectedSemester;

    [ObservableProperty]
    private string? schedulePdfPath;

    [ObservableProperty]
    private string? errorMessage;

    public GuideViewModel(AppState appState, INavigationService navigationService)
    {
        _appState = appState;
        _navigationService = navigationService;
    }

    [RelayCommand]
    private void Next()
    {
        ErrorMessage = ValidateStep();
        if (ErrorMessage != null)
            return;

        if (CurrentStep < totalSteps)
        {
            CurrentStep++;
            return;
        }

        FinishGuide();
    }

    [RelayCommand]
    private void Back()
    {
        if (CurrentStep > 1)
            CurrentStep--;
    }

    [RelayCommand]
    private void Skip()
    {
        FinishGuide();
    }

    private string? ValidateStep()
    {
        if (CurrentStep == 1)
        {
            if (string.IsNullOrWhiteSpace(SelectedStudyProgram))
                return "Bitte wähle deinen Studiengang aus.";

            if (SelectedSemester is null)
                return "Bitte wähle dein aktuelles Semester aus.";
        }

        return null;
    }

    private void FinishGuide()
    {
        _navigationService.NavigateTo<ShellViewModel>();
    }
}
