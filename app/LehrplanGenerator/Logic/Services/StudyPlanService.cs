using System;
using System.Linq;
using System.Xml.Linq;
using LehrplanGenerator.Logic.Models;

namespace LehrplanGenerator.Logic.Services;

public class StudyPlanService
{
    public StudyPlan LoadFromXml(string path)
    {
        var doc = XDocument.Load(path);

        var title = doc.Root?.Element("Title")?.Value ?? "Unbenannt";

        var entries = doc.Descendants("Entry")
                         .Select(e => new StudyEntry(
                             Guid.NewGuid(),
                             Guid.NewGuid(),
                             DateTime.Parse(e.Element("Date")!.Value),
                             int.Parse(e.Element("Hour")!.Value),
                             e.Element("Topic")!.Value,
                             int.Parse(e.Element("DurationMinutes")!.Value)
                         ))
                         .ToList();

        return new StudyPlan(
            Guid.NewGuid(),
            Guid.NewGuid(),
            title,
            entries,
            doc.ToString(),
            DateTime.Now.ToString("s")
        );
    }

    /*
    public WorkShift GetWorkShift(StudyEntry entry, DateTime day)
        {
            var start = day.Date.AddHours(entry.Hour);
            var end = start.AddMinutes(entry.DurationMinutes);
            return new WorkShift(start, end);
        }

        */
}
