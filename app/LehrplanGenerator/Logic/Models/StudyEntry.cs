using System;

namespace LehrplanGenerator.Logic.Models;

public record StudyEntry(
    Guid EntryId,
    Guid StudyPlanId,
    DateTime Date,
    int Hour,
    string Topic,
    int DurationMinutes
)
{

    public DateTime Start => DateTime.Today.AddHours(Hour);
    public DateTime End => Start.AddMinutes(DurationMinutes);

    public string DayFormatted => Date.ToString("dd.MM");
    public string StartFormatted => Start.ToString("HH:mm");
    public string EndFormatted => End.ToString("HH:mm");
    public string DurationFormatted => $"{DurationMinutes} Min";
};