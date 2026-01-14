using CommunityToolkit.Mvvm.ComponentModel;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Logic.Services;
using System.Collections.ObjectModel;
using LehrplanGenerator.Views.Windows;
using System.Linq;
using LehrplanGenerator.Logic.State;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.ViewModels.Main;
using LehrplanGenerator.Logic.AI;
using System;
using System.Threading.Tasks;

namespace LehrplanGenerator.ViewModels.StudyPlan;

public partial class StudyPlanViewModel : ViewModelBase
{
    private readonly StudyPlanService _service;
    private readonly INavigationService _navigationService;
    private readonly AppState _appState;

    [ObservableProperty]
    private string _topic = string.Empty;

    [ObservableProperty]
    private ObservableCollection<DayPlanViewModel> _days = new();

    [ObservableProperty]
    private bool _isLoading = true;

    [ObservableProperty]
    private string? _errorMessage;

    public StudyPlanViewModel(INavigationService navigationService, AppState appState)
    {
        _navigationService = navigationService;
        _appState = appState;
        _service = new StudyPlanService(new Logic.Models.StudyPlan(string.Empty, new()));
        
        // Lade Studienplan asynchron
        _ = LoadStudyPlanAsync();
    }

    private async Task LoadStudyPlanAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            // Prüfe, ob bereits ein Plan im AppState existiert
            var studyPlan = _appState.CurrentStudyPlan;

            // Falls nicht, erstelle einen neuen
            if (studyPlan == null)
            {
                studyPlan = await _appState.AiService.CreateStudyPlanAsync();
                if (studyPlan != null)
                {
                    _appState.CurrentStudyPlan = studyPlan;
                }
            }

            if (studyPlan != null)
            {
                // Verarbeite und sortiere den Plan
                

                Topic = studyPlan.Topic;
                Days = new ObservableCollection<DayPlanViewModel>(
                   studyPlan.Days.Select(d => new DayPlanViewModel(d))
                );
            }
            else
            {
                ErrorMessage = "Fehler beim Erstellen des Studienplans. Bitte versuchen Sie es erneut.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Fehler: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        // Erzwinge Neuerstellung des Plans
        _appState.CurrentStudyPlan = null;
        await LoadStudyPlanAsync();
    }

    [RelayCommand]
    private void GoBack()
    {
        _navigationService.NavigateTo<MainViewModel>();
    }
}
