using System;
using System.Collections.ObjectModel;
using System.Linq;
using LehrplanGenerator.Logic.Models;

namespace LehrplanGenerator.ViewModels.StudyPlan;

public class DayPlanViewModel
{
   public string DisplayDate => Date.ToString("dddd, dd.MM.yyyy"); 
    public DateTime Date { get; } 
    public ObservableCollection<TaskItemViewModel> Tasks { get; }

    public DayPlanViewModel(DayPlan day)
    {
        Date = day.Date;
        Tasks = new ObservableCollection<TaskItemViewModel>(
            day.Tasks.Select(t => new TaskItemViewModel(t))
        );
    }
}