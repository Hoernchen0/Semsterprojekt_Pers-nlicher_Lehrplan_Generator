using System;
using LehrplanGenerator.Logic.Models;
using System.Linq;

namespace LehrplanGenerator.Logic.Services;

public class StudyPlanService
{

    private readonly StudyPlan _studyPlan;

    public StudyPlanService(StudyPlan studyPlan)
    {
        _studyPlan = studyPlan;
    }

    public StudyPlan GetStudyPlan()
    {
        _studyPlan.Days.Sort((a, b) => a.Date.CompareTo(b.Date));

        foreach (var day in _studyPlan.Days)
        {
            var sorted = day.Tasks
                .OrderBy(t => TimeSpan.Parse(t.StartTime))
                .ToList();

            day.Tasks.Clear();
            foreach (var t in sorted)
                day.Tasks.Add(t);
        }

        return _studyPlan;
    }
}
