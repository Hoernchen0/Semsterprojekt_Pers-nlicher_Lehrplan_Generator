using System;
using System.Collections.ObjectModel;
using System.Linq;
using LehrplanGenerator.Logic.Models;

namespace LehrplanGenerator.ViewModels.StudyPlan;

public class DayPlanViewModel
{
    public string Date { get; }
    public string DisplayDate => DateTime.Parse(Date).ToString("dddd, dd.MM.yyyy");
    public ObservableCollection<TaskItemViewModel> Tasks { get; }

    public DayPlanViewModel(DayPlan day)
    {
        Date = day.Day;
        Tasks = new ObservableCollection<TaskItemViewModel>(
            day.Tasks.Select(t => new TaskItemViewModel(t))
        );
    }
}