using CommunityToolkit.Mvvm.ComponentModel;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Logic.Services;
using System.Collections.ObjectModel;
using LehrplanGenerator.Views.Windows;
using System.Linq;
using LehrplanGenerator.Logic.State;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.ViewModels.Main;

namespace LehrplanGenerator.ViewModels.StudyPlan;

public partial class StudyPlanViewModel : ViewModelBase
{

    private readonly StudyPlanService _service;

    private readonly INavigationService _navigationService;
    private readonly AppState _appState;

    public string Topic { get; }
    public ObservableCollection<DayPlanViewModel> Days { get; }



    public StudyPlanViewModel(INavigationService navigationService, AppState appState)
    {
        _navigationService = navigationService;
        _appState = appState;

        var demoPlan = StudyPlanFactory.CreateDemoPlan();
        _service = new StudyPlanService(demoPlan);

        var plan = _service.GetStudyPlan();

        Topic = plan.Topic;
        Days = new ObservableCollection<DayPlanViewModel>(
            plan.Days.Select(d => new DayPlanViewModel(d))
        );
    }

    [RelayCommand]
    private void GoBack()
    {
        _navigationService.NavigateTo<MainViewModel>();
    }
}
