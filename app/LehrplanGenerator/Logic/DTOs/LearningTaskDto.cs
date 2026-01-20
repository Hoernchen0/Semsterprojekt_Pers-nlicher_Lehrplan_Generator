using System;

namespace LehrplanGenerator.Logic.DTOs;

public class LearningTaskDto
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string StartTime { get; init; } = string.Empty;
    public string EndTime { get; init; } = string.Empty;

    public bool IsDone { get; init; }
}
