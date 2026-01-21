using LehrplanGenerator.Logic.Models;
using System;

namespace LehrplanGenerator.ViewModels.StudyPlan;

public class TaskItemViewModel : ViewModelBase
{
    public Guid Id { get; }
    public string Title { get; }
    public string Time { get; }
    public string Description { get; }
    public bool IsDone { get; }

    public TaskItemViewModel(LearningProgressEntity entity)
    {
        Id = entity.Id;
        Title = entity.ModuleName;
        Description = entity.Chapter;
        Time =
            $"{entity.PlannedStart.ToLocalTime():HH:mm} – {entity.PlannedEnd.ToLocalTime():HH:mm}";
        IsDone = entity.IsCompleted;
    }
}
