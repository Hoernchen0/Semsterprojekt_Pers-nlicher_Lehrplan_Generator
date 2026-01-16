using System.ComponentModel;
using LehrplanGenerator.Logic.Models;

namespace LehrplanGenerator.ViewModels.StudyPlan;

public class TaskItemViewModel : ViewModelBase
{
    public string Title { get; }
    public string Time => $"{StartTime} – {EndTime}";
    public string StartTime { get; }
    public string EndTime { get; }
    public string Description { get; }

    private bool _isDone;
    public bool IsDone
    {
        get => _isDone;
        set => SetProperty(ref _isDone, value);
    }

    public TaskItemViewModel(TaskItem task)
    {
        Title = task.Title;
        StartTime = task.StartTime;
        EndTime = task.EndTime;
        Description = task.Description;
        _isDone = task.IsDone;

    }
}
