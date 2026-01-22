using OpenAI.Chat;
using System.Text.Json.Serialization;
using OpenAI;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;
using LehrplanGenerator.Logic.Models;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using LehrplanGenerator.Logic.Services;
using System.Linq;

namespace LehrplanGenerator.Logic.AI;

public class StudyPlanGeneratorService
{
    private readonly OpenAIClient? _client;
    private readonly ChatServiceDb _chatServiceDb;
    private readonly List<Message> _conversation;
    private const string ModelName = "gpt-5-chat";
    private Guid? _currentUserId;
    private Guid? _currentSessionId;

    public StudyPlanGeneratorService(ChatServiceDb chatServiceDb)
    {
        _chatServiceDb = chatServiceDb;
        
        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
        {
            _client = null;
        }
        else
        {
            var settings = new OpenAISettings(resourceName: endpoint, deploymentId: ModelName, apiVersion: "2025-03-01-preview");
            _client = new OpenAIClient(apiKey, settings);
        }

        _conversation = new List<Message>
        {
            new Message(Role.System, """
                Hilf dem Nutzer einen Lernplan zu erstellen und seine Fragen zu Themen zu beantworten.
                Du darfst den Lernplan unter keinen Umständen im Chat den ausgeben...
                Du kannst ihn auch fragen welche Themen er besonders intensiv wiederholen will, das ist sehr wichtig für die richtige Zeitaufteilung im Lernplan.
                Für die Erstellung ist auch wichtig zu fragen zu welchen Zeiten der Nutzer lernen möchte.
                Plane die Themen über die Zeit so, dass Wiederholungen für schwierige Themen vorgesehen sind.
                Lege zuerst die Themen an, die für das Verständnis der anderen Themen am wichtigsten sind.
                Überfordere den Nutzer nicht und halte dich kurz.
                Sage dem Nutzer nicht dass du den Lernplan im Hintergrund erstellen wirst, wenn du genug Informationen hast, sag ihm, dass er jederzeit den Lernplan über das Plus links erstellen kann.
                Versetze dich in die Rolle des Systems und gehe nicht auf Smalltalk ein.
                Konzentriere dich auf die Erstellung des Lernplans.
                Benutze keine Emojis im Chat.
                """)
        };
    }

    // Initialisiert die Conversation aus der Datenbank für einen User/Session
    public async Task InitializeConversationAsync(Guid userId, Guid sessionId)
    {
        _currentUserId = userId;
        _currentSessionId = sessionId;

        // Lade alle Messages aus der Datenbank
        var dbMessages = await _chatServiceDb.GetSessionMessagesAsync(sessionId, userId);
        
        // Behalte nur die System-Prompt-Message aus dem Konstruktor
        var systemPrompt = _conversation.FirstOrDefault(m => m.Role == Role.System);
        _conversation.Clear();
        
        if (systemPrompt != null)
            _conversation.Add(systemPrompt);

        // Füge nur User- und AI-Messages zur Conversation hinzu
        // System-Messages aus der DB werden ignoriert (sind nur für die UI)
        foreach (var msg in dbMessages.OrderBy(m => m.CreatedAt))
        {
            var sender = msg.Sender.ToLower();
            
            if (sender == "user")
            {
                _conversation.Add(new Message(Role.User, msg.Text));
            }
            else if (sender == "ai")
            {
                _conversation.Add(new Message(Role.Assistant, msg.Text));
            }
            else if (sender == "pdf_context")
            {
                // PDF-Kontext wird als User-Message für die AI hinzugefügt
                _conversation.Add(new Message(Role.User, msg.Text));
            }
            // "system" messages werden ignoriert - sind nur UI-Meldungen
        }
    }

    public async Task<StudyPlan?> CreateStudyPlanAsync()
    {
        if (_client == null)
        {
            Console.WriteLine("Fehler: Azure OpenAI ist nicht konfiguriert.");
            return null;
        }

        var heute = DateTime.Now.ToString("dd.MM.yyyy");

        var systemMessage = new Message(Role.System,
            "day soll das Format dd.MM.yyyy haben und start_time und end_time das Format HH:mm. " +
            $"Der erste Tag darf nicht vor dem heutigen Datum liegen: {heute}, gib den Lernplan ausschließlich im JSON Format zurück. ");

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
        }
        catch (Exception ex)
        {
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

        // Prüfe ob User und Session verfügbar sind
        if (_currentUserId == null || _currentSessionId == null)
        {
            Console.WriteLine("Fehler: Keine aktive Session für PDF-Upload.");
            return false;
        }

        try
        {
            string fileName = Path.GetFileName(pdfPath);

            // Extrahiere den Text aus der PDF
            string extractedText = ExtractTextFromPdf(pdfPath);

            if (string.IsNullOrWhiteSpace(extractedText))
            {
                Console.WriteLine("Fehler: Kein Text aus der PDF extrahiert.");
                return false;
            }

            // Formatiere die Nachricht mit dem PDF-Inhalt
            var pdfMessage = $"Ich habe die PDF-Datei '{fileName}' hochgeladen. Hier ist der Inhalt:\n\n{extractedText}\n\n" +
                           "Bitte nutze diesen Inhalt als Grundlage für Zusammenfassungen und den Lernplan.";

            // Füge den extrahierten Text als Nachricht zur Konversation hinzu
            _conversation.Add(new Message(Role.User, pdfMessage));

            // Speichere die PDF-Message in der Datenbank mit speziellem Sender "PDF_Context"
            // Dieser Sender wird beim UI-Laden gefiltert, aber für die AI-Conversation geladen
            await _chatServiceDb.SaveMessageAsync(
                _currentSessionId.Value,
                _currentUserId.Value,
                "PDF_Context",
                pdfMessage
            );

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Verarbeiten der PDF: {ex.Message}");
            return false;
        }
    }

    private string ExtractTextFromPdf(string pdfPath)
    {
        try
        {
            using (var pdfReader = new PdfReader(pdfPath))
            using (var pdfDocument = new PdfDocument(pdfReader))
            {
                var textBuilder = new System.Text.StringBuilder();

                for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                {
                    var page = pdfDocument.GetPage(i);
                    string pageText = PdfTextExtractor.GetTextFromPage(page);
                    textBuilder.AppendLine(pageText);
                    textBuilder.AppendLine(); // Leerzeile zwischen Seiten
                }

                return textBuilder.ToString();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Extrahieren des PDF-Textes: {ex.Message}");
            throw;
        }
    }
    
    public async Task<string?> AskGptAsync(string userInput)
    {
        if (_client == null)
        {
            Console.WriteLine("Fehler: Azure OpenAI ist nicht konfiguriert.");
            return null;
        }

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

    // Gibt die aktuelle Conversation-History zurück (ohne System-Message)
    public List<Message> GetConversationHistory()
    {
        return _conversation.Where(m => m.Role != Role.System).ToList();
    }

    // Löscht die Conversation (behält nur System-Message)
    public void ClearConversation()
    {
        var systemMessage = _conversation.FirstOrDefault(m => m.Role == Role.System);
        _conversation.Clear();
        if (systemMessage != null)
            _conversation.Add(systemMessage);
    }
}