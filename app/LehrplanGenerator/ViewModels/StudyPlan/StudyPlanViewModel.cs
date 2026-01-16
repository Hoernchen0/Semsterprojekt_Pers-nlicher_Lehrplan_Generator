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
        
        // Lade Daten direkt aus AppState
        LoadFromAppState();
    }

    private void LoadFromAppState()
    {
        var studyPlan = _appState.CurrentStudyPlan;

        if (studyPlan != null)
        {
            Topic = studyPlan.Topic;
            Days = new ObservableCollection<DayPlanViewModel>(
               studyPlan.Days.Select(d => new DayPlanViewModel(d))
            );
            IsLoading = false;
        }
        else
        {
            ErrorMessage = "Kein Lernplan vorhanden. Bitte erstellen Sie zuerst einen Plan über den Chat.";
            IsLoading = false;
        }
    }

    private async Task LoadStudyPlanAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            // Prüfe, ob bereits ein Plan im AppState existiert
            var studyPlan = _appState.CurrentStudyPlan;

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
                ErrorMessage = "Kein Lernplan vorhanden. Bitte erstellen Sie zuerst einen Plan über den Chat.";
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
    private void GoBack()
    {
        _navigationService.NavigateTo<MainViewModel>();
    }
}
