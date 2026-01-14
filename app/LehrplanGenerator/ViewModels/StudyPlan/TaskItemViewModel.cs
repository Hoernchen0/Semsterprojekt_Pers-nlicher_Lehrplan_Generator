using LehrplanGenerator.Logic.Models;

namespace LehrplanGenerator.ViewModels.StudyPlan;
public class TaskItemViewModel
{
    public string Title { get; }
    public string Time => $"{StartTime} – {EndTime}";
    public string StartTime { get; }
    public string EndTime { get; }
    public string Description { get; }

    public TaskItemViewModel(TaskItem task)
    {
        Title = task.Title;
        StartTime = task.StartTime;
        EndTime = task.EndTime;
        Description = task.Description;
    }
}