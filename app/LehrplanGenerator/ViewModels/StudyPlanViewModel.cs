using CommunityToolkit.Mvvm.ComponentModel;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Logic.Services;
using System.Collections.ObjectModel;
using LehrplanGenerator.Views.Windows;
using System.Linq;


namespace LehrplanGenerator.ViewModels;

public partial class StudyPlanViewModel : ObservableObject
{

    private readonly MainWindow _mainWindow;
    private readonly StudyPlanService _service;

    public string Topic { get; }
    public ObservableCollection<DayPlanViewModel> Days { get; }

    public StudyPlanViewModel(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;

        var demoPlan = StudyPlanFactory.CreateDemoPlan();
        _service = new StudyPlanService(demoPlan);

        var plan = _service.GetStudyPlan();

        Topic = plan.Topic;
        Days = new ObservableCollection<DayPlanViewModel>(
            plan.Days.Select(d => new DayPlanViewModel(d))
        );
    }
}
