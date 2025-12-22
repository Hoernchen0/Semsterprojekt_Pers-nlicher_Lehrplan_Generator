using System.Text.Json.Serialization;
public class TaskItem
{

    [JsonInclude]
    [JsonPropertyName("title")]
    public string Title { get; private set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("start_time")]
    public string StartTime { get; private set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("end_time")]
    public string EndTime { get; private set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("description")]
    public string Description { get; private set; } = string.Empty;


    public TaskItem(string title, string start, string end, string description)
    {
        Title = title;
        StartTime = start;
        EndTime = end;
        Description = description;
    }

    private TaskItem()
    {
        Title = string.Empty;
        StartTime = string.Empty;
        EndTime = string.Empty;
        Description = string.Empty;
    }
}