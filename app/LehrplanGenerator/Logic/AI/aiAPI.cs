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

namespace LehrplanGenerator.Logic.AI;

public class StudyPlanGeneratorService
{
    private readonly OpenAIClient? _client;
    private readonly List<Message> _conversation;
    private const string ModelName = "gpt-5-chat";

    public StudyPlanGeneratorService()
    {
        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        
        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
        {
            _client = null;
            Console.WriteLine("⚠ Azure OpenAI nicht konfiguriert (fehlende Umgebungsvariablen)");
        }
        else
        {
            try
            {
                var settings = new OpenAISettings(domain: endpoint);
                _client = new OpenAIClient(apiKey, settings);
                Console.WriteLine("✓ Azure OpenAI verbunden");
            }
            catch (Exception ex)
            {
                _client = null;
                Console.WriteLine($"❌ Azure OpenAI Fehler: {ex.Message}");
            }
        }

        _conversation = new List<Message>
        {
            new Message(Role.System, "Hilf dem Nutzer einen Lernplan zu erstellen und"
                                                   + "seine Fragen zu Themen zu beantworten."
                                 + "Du darfst nicht einfach im Chat den Lernplan ausgeben..."
                                +"Du kannst ihn auch fragen welche Themen er besonders "
                                +  "intensiv wiederholen will, das ist sehr wichtig für die richtige Zeitaufteilung"
                                +  "im Lernplan. Für die Erstellung ist auch wichtig zu fragen zu welchen Zeiten"
                                +  "der Nutzer lernen möchte. Plane die Themen über die Zeit so, "
                                +  "dass Wiederholungen für schwierige Themen vorgesehen sind. Lege zuerst die Themen an,"
                               +   " die für das Verständnis der anderen Themen am wichtigsten sind.")
        };
    }

    public async Task<StudyPlan?> CreateStudyPlanAsync()
    {
        // Wenn keine KI konfiguriert ist, gib Fehler zurück
        if (_client == null)
        {
            Console.WriteLine("❌ Fehler: Azure OpenAI ist nicht konfiguriert.");
            Console.WriteLine("   AZURE_OPENAI_ENDPOINT und OPENAI_API_KEY müssen gesetzt sein.");
            return null;
        }

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
                Console.WriteLine("❌ Fehler: Antwort konnte nicht in StudyPlan geparst werden.");
                return null;
            }

            Console.WriteLine($"✓ Lernplan erfolgreich erstellt: {studyPlan.Topic}");
            return studyPlan;
        }    
        catch (Exception ex)
        {
            Console.WriteLine($"❌ API error bei Lernplan-Erstellung: {ex.Message}");
            return null;
        }
    }
    public async Task<bool> UploadPdfAsync(string pdfPath)
    {
        if (_client == null)
        {
            Console.WriteLine("❌ Fehler: Azure OpenAI ist nicht konfiguriert. PDF kann nicht verarbeitet werden.");
            return false;
        }

        if (!File.Exists(pdfPath))
        {
            Console.WriteLine("❌ Datei existiert nicht!");
            return false;
        }

        try
        {
            string fileName = Path.GetFileName(pdfPath);

            // Extrahiere den Text aus der PDF
            string extractedText = ExtractTextFromPdf(pdfPath);

            if (string.IsNullOrWhiteSpace(extractedText))
            {
                Console.WriteLine("❌ Fehler: Kein Text aus der PDF extrahiert.");
                return false;
            }

            // Füge den extrahierten Text als Nachricht zur Konversation hinzu
            _conversation.Add(new Message(Role.User, 
                $"Ich habe die PDF-Datei '{fileName}' hochgeladen. Hier ist der Inhalt:\n\n{extractedText}\n\n" +
                "Bitte nutze diesen Inhalt als Grundlage für Zusammenfassungen und den Lernplan."));
            
            _conversation.Add(new Message(Role.System, 
                "Nutze den bereitgestellten PDF-Inhalt als Grundlage für Zusammenfassungen und den Lernplan."
            ));

            Console.WriteLine($"✓ PDF-Text erfolgreich extrahiert und zur Konversation hinzugefügt.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Fehler beim Verarbeiten der PDF: {ex.Message}");
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
    // Wenn keine KI konfiguriert ist, gib eine Fehler zurück
    if (_client == null)
    {
        Console.WriteLine("❌ AskGptAsync: Azure OpenAI ist nicht konfiguriert!");
        return "Fehler: Azure OpenAI ist nicht konfiguriert. Bitte stelle sicher, dass AZURE_OPENAI_ENDPOINT und OPENAI_API_KEY gesetzt sind.";
    }

    Console.WriteLine($"📨 User-Input: '{userInput}'");
    
    // Nachricht als User-Message hinzufügen
    _conversation.Add(new Message(Role.User, userInput));
    Console.WriteLine($"✓ Message zur Conversation hinzugefügt ({_conversation.Count} messages gesamt)");

    try
    {
        Console.WriteLine($"🔄 Rufe Azure OpenAI API auf (Model: {ModelName})...");
        
        // Response von der API abrufen
        var chatResponse = await _client.ChatEndpoint.GetCompletionAsync(
            new ChatRequest(_conversation, model: ModelName)
        );

        Console.WriteLine($"✓ Response erhalten");

        var assistantMessage = chatResponse.FirstChoice.Message;
        
        if (assistantMessage == null || string.IsNullOrEmpty(assistantMessage.Content))
        {
            Console.WriteLine("⚠ Leere Response von API");
            return "Ich habe keine gültige Antwort erhalten. Bitte versuche es erneut.";
        }

        var responseContent = assistantMessage.Content.ToString();
        Console.WriteLine($"✓ KI-Antwort: {responseContent.Substring(0, Math.Min(100, responseContent.Length))}...");
        
        // Komplette Message hinzufügen
        _conversation.Add(assistantMessage);

        return responseContent;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ API FEHLER: {ex.GetType().Name}");
        Console.WriteLine($"❌ Nachricht: {ex.Message}");
        Console.WriteLine($"❌ Stack: {ex.StackTrace}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"❌ Inner: {ex.InnerException.Message}");
        }
        return null;
    }
}
}