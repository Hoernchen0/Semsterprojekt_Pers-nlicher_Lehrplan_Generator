using System;
using System.Collections.Generic;

namespace LehrplanGenerator.Logic.Models;

public record StudyPlan(
    Guid StudyPlanId,
    Guid UserId,
    string Title,
    List<StudyEntry> Entries,
    string xmlRaw,
    string createdAt
);