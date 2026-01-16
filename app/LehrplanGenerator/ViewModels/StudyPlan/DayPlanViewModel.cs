using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using LehrplanGenerator.Logic.Models;

namespace LehrplanGenerator.ViewModels.StudyPlan;

public class DayPlanViewModel : ViewModelBase
{
    public string Date { get; }
    public string DisplayDate => DateTime.Parse(Date).ToString("dddd, dd.MM.yyyy");
    public ObservableCollection<TaskItemViewModel> Tasks { get; }

    private bool _isDone;
    public bool IsDone
    {
        get => _isDone;
        set
        {
            if (SetProperty(ref _isDone, value))
            {
                foreach (var task in Tasks)
                    task.IsDone = value;
            }
        }
    }

    public DayPlanViewModel(DayPlan day)
    {
        Date = day.Day;

        Tasks = new ObservableCollection<TaskItemViewModel>(
            day.Tasks.Select(t => new TaskItemViewModel(t))
        );

        HookTasks();
        Tasks.CollectionChanged += (_, __) => HookTasks();
    }

    private void HookTasks()
    {
        foreach (var task in Tasks)
        {
            task.PropertyChanged -= Task_PropertyChanged;
            task.PropertyChanged += Task_PropertyChanged;
        }
    }

    private void Task_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TaskItemViewModel.IsDone))
        {
            _isDone = Tasks.All(t => t.IsDone);
            OnPropertyChanged(nameof(IsDone));
        }
    }
}
