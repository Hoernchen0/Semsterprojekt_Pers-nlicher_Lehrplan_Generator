using System;

namespace LehrplanGenerator.Logic.Models;

public record StudyEntry(
    Guid EntryId,
    Guid StudyPlanId,
    int Hour,
    string Topic,
    int DurationMinutes
);