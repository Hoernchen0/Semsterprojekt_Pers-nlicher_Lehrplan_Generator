using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;

namespace LehrplanGenerator.ViewModels.StudyPlan;

public class DayPlanViewModel : ViewModelBase
{
    public string Date { get; }

    public string DisplayDate =>
        DateTime.ParseExact(Date, "dd.MM.yyyy", CultureInfo.InvariantCulture)
            .ToString("dddd, dd.MM.yyyy");

    public ObservableCollection<TaskItemViewModel> Tasks { get; }

    public bool IsDone => Tasks.All(t => t.IsDone);

    public DayPlanViewModel(string date, IEnumerable<TaskItemViewModel> tasks)
    {
        Date = date;
        Tasks = new ObservableCollection<TaskItemViewModel>(tasks);
    }
}
