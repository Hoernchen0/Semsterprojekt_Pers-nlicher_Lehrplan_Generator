using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using OpenAI.Chat;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using OpenAI;
namespace LehrplanGenerator.Logic.Services
{
public class TaskItem
{
    [JsonInclude]
    [JsonPropertyName("title")]
    public string Title { get; private set; }

    [JsonInclude]
    [JsonPropertyName("start_time")]
    public string StartTime { get; private set; }

    [JsonInclude]
    [JsonPropertyName("end_time")]
    public string EndTime { get; private set; }

    [JsonInclude]
    [JsonPropertyName("description")]
    public string Description { get; private set; }
}

public class DayPlan
{
    [JsonInclude]
    [JsonPropertyName("day")]
    public string Day { get; private set; }

    [JsonInclude]
    [JsonPropertyName("tasks")]
    public List<TaskItem> Tasks { get; private set; }
}

public class StudyPlan
{
    [JsonInclude]
    [JsonPropertyName("topic")]
    public string Topic { get; private set; }

    [JsonInclude]
    [JsonPropertyName("days")]
    public List<DayPlan> Days { get; private set; }
}

public class StudyPlanGeneratorService
{
    private readonly OpenAIClient _client;
    private readonly List<Message> _conversation;
    private const string ModelName = "gpt-5-chat";

    public StudyPlanGeneratorService()
    {
        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
                      ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                     ?? throw new InvalidOperationException("OPENAI_API_KEY is not set.");
        var settings = new OpenAISettings(resourceName: endpoint, deploymentId: ModelName, apiVersion: "2025-03-01-preview");
        _client = new OpenAIClient(apiKey,settings); 

        _conversation = new List<Message>
        {
            new Message(Role.System, "Hilf dem Nutzer einen Lernplan zu erstellen und"
                                                   + "seine Fragen zu Themen zu beantworten."
                                 + "Du sollst im nicht einfach im Chat den Lernplan ausgeben..."
                               +   "Du kannst den Nutzer auch fragen ob du im selbst von dir erstellte Praxisübungen"
                                +  "erstellen sollst. Du kannst ihn auch fragen welche Themen er besonders "
                                +  "intensiv wiederholen will, das ist sehr wichtig für die richtige Zeitaufteilung"
                                +  "im Lernplan. Für die Erstellung ist auch wichtig zu fragen zu welchen Zeiten"
                                +  "der Nutzer lernen möchte. Plane die Themen über die 2 Wochen so, "
                                +  "dass Wiederholungen für schwierige Themen vorgesehen sind. Lege zuerst die Themen an,"
                               +   " die für das Verständnis der anderen Themen am wichtigsten sind.")
        };
    }

    public async Task<StudyPlan?> CreateJsonAsync()
    {
        var heute = DateTime.Now.ToString("dd.MM.yyyy");

        var systemMessage = new Message(Role.System,
            "Erstelle einen Lernplan und gib ihn NUR als JSON zurück, der die Struktur StudyPlan hat. " +
            "day soll das Format dd.MM.yyyy haben und start_time und end_time das Format HH:mm. " +
            $"Der erste Tag darf nicht vor dem heutigen Datum liegen: {heute}");

        var messages = new List<Message> { systemMessage };
        messages.AddRange(_conversation);

try
    {
            // Typisierte Response abrufen
            var (studyPlan, chatResponse) = await _client.ChatEndpoint.GetCompletionAsync<StudyPlan>(
                new ChatRequest(messages, model: ModelName)
            );

            if (studyPlan == null)
            {
                Console.WriteLine("Fehler: Antwort konnte nicht in StudyPlan geparst werden.");
                return null;
            }
                // direkt in deiner UI verwenden
        foreach (var day in studyPlan.Days)
        {
        Console.WriteLine($"Tag: {day.Day}");
        foreach (var task in day.Tasks)
        {
            Console.WriteLine($"- {task.Title} ({task.StartTime}-{task.EndTime})");
        }
    }
        
        return studyPlan;
    }    catch (Exception ex){
        Console.WriteLine($"API error: {ex.Message}");
        return null;
    }
    
    }
    public async Task UploadPdfAsync(string pdfPath)
    {
    if (!File.Exists(pdfPath))
    {
        Console.WriteLine("Datei existiert nicht!");
        return;
    }

    // Hinweis: Aktuelles SDK unterstützt noch kein direktes File-Upload wie in Python
    // Wir fügen die Info zur PDF einfach als Textnachricht in die Konversation ein
    _conversation.Add(new Message(Role.User, "Hier ist meine PDF."));
    _conversation.Add(new Message(Role.System, 
        "Nutze die PDF als Grundlage für Zusammenfassungen und den Lernplan."
    ));

    Console.WriteLine("PDF-Hinweis zur Konversation hinzugefügt.");
    }
public async Task<string?> AskGptAsync(string userInput)
{
    // Nachricht als User-Message hinzufügen
    _conversation.Add(new Message(Role.User, userInput));

    try
    {
        // Response von der API abrufen
        var chatResponse = await _client.ChatEndpoint.GetCompletionAsync(
            new ChatRequest(_conversation, model: ModelName)
        );

        var assistantMessage = chatResponse.FirstChoice.Message;
        
        // Komplette Message hinzufügen
        _conversation.Add(assistantMessage);

        Console.WriteLine(assistantMessage.Content.ToString());
        return assistantMessage.Content.ToString();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"API error: {ex.Message}");
        return null;
    }
}

}
}