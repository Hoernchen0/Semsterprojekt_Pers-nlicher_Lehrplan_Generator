using System.Text.Json.Serialization;
using System.Collections.Generic;
using System;
namespace LehrplanGenerator.Logic.Models;


public class DayPlan
{
    [JsonInclude]
    [JsonPropertyName("day")]
    public DateTime Date { get; private set; }

    [JsonInclude]
    [JsonPropertyName("tasks")]
    public List<TaskItem> Tasks { get; private set; } = new List<TaskItem>();


    public DayPlan(DateTime date, List<TaskItem> tasks)
    {
        Date = date;
        Tasks = tasks;
    }

    private DayPlan()
    {
        Tasks = new List<TaskItem>();
    }
}