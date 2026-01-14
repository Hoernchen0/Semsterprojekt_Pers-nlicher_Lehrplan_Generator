using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace LehrplanGenerator.Logic.Models;

public class StudyPlan
{

    //Guid StudyPlanId;
    //Guid UserId;
    //string createdAt

    [JsonInclude]
    [JsonPropertyName("topic")]
    public string Topic { get; private set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("days")]
    public List<DayPlan> Days { get; private set; } = new List<DayPlan>();

    public StudyPlan(string topic, List<DayPlan> days)
    {
        Topic = topic;
        Days = days;
    }

    // Parameterloser Konstruktor muss public sein für JSON-Deserialisierung
    public StudyPlan()
    {
        Topic = string.Empty;
        Days = new List<DayPlan>();
    }
}