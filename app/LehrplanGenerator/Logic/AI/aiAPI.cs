using OpenAI.Chat;
using System.Text.Json.Serialization;
using OpenAI;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;
using OpenAI.Files;
using LehrplanGenerator.Logic.Models;

namespace LehrplanGenerator.Logic.AI;

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
                                 + "Du darfst nicht einfach im Chat den Lernplan ausgeben..."
                               +   "Du kannst den Nutzer auch fragen ob du im selbst von dir erstellte Praxisübungen"
                                +  "erstellen sollst. Du kannst ihn auch fragen welche Themen er besonders "
                                +  "intensiv wiederholen will, das ist sehr wichtig für die richtige Zeitaufteilung"
                                +  "im Lernplan. Für die Erstellung ist auch wichtig zu fragen zu welchen Zeiten"
                                +  "der Nutzer lernen möchte. Plane die Themen über die Zeit so, "
                                +  "dass Wiederholungen für schwierige Themen vorgesehen sind. Lege zuerst die Themen an,"
                               +   " die für das Verständnis der anderen Themen am wichtigsten sind.")
        };
    }

    public async Task<StudyPlan?> CreateStudyPlanAsync()
    {
        var heute = DateTime.Now.ToString("dd.MM.yyyy");

        var systemMessage = new Message(Role.System,
            "day soll das Format dd.MM.yyyy haben und start_time und end_time das Format HH:mm. " +
            $"Der erste Tag darf nicht vor dem heutigen Datum liegen: {heute}, gib den Lernplan ausschließlich im JSON Format zurück. " );

        var messages = new List<Message> { systemMessage };
        messages.AddRange(_conversation);

try
    {
            // Typisierte Response abrufen
            var (studyPlan, chatResponse) = await _client.ChatEndpoint.GetCompletionAsync<StudyPlan>(
                new ChatRequest(messages, model: ModelName) //automatische json deserialisierung
            );

            if (studyPlan == null)
            {
                Console.WriteLine("Fehler: Antwort konnte nicht in StudyPlan geparst werden.");
                return null;
            }

       
        return studyPlan;
    }    catch (Exception ex){
        Console.WriteLine($"API error: {ex.Message}");
        return null;
    }
    
    }
    public async Task<bool> UploadPdfAsync(string pdfPath)
    {
        if (!File.Exists(pdfPath))
        {
            Console.WriteLine("Datei existiert nicht!");
            return false;
        }

        try
        {
            string fileName = Path.GetFileName(pdfPath);

            // Lade die Datei über die Files API hoch
            var uploadedFile = await _client.FilesEndpoint.UploadFileAsync(pdfPath, FilePurpose.Assistants);

            // Füge eine Nachricht mit Dateireferenz hinzu
            _conversation.Add(new Message(Role.User, 
                $"Ich habe die PDF-Datei '{fileName}' hochgeladen (File ID: {uploadedFile.Id}). " +
                "Bitte nutze diese als Grundlage für Zusammenfassungen und den Lernplan."));
            
            _conversation.Add(new Message(Role.System, 
                "Nutze die hochgeladene PDF als Grundlage für Zusammenfassungen und den Lernplan."
            ));

            Console.WriteLine($"PDF erfolgreich hochgeladen: {uploadedFile.Id}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Hochladen der PDF: {ex.Message}");
            return false;
        }
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

        return assistantMessage.Content.ToString();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"API error: {ex.Message}");
        return null;
    }
}
}