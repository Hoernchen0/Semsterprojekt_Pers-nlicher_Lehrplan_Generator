using System.Text.Json.Serialization;
using System.Collections.Generic;
using System;
namespace LehrplanGenerator.Logic.Models;


public class DayPlan
{
    [JsonInclude]
    [JsonPropertyName("day")]
    public string Day { get; private set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("tasks")]
    public List<TaskItem> Tasks { get; private set; } = new List<TaskItem>();


    public DayPlan(string day, List<TaskItem> tasks)
    {
        Day = day;
        Tasks = tasks;
    }

    // Parameterloser Konstruktor muss public sein für JSON-Deserialisierung
    public DayPlan()
    {
        Tasks = new List<TaskItem>();
    }
}