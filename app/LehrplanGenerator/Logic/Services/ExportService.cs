using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Data.Repositories;
using LehrplanGenerator.Logic.State;

namespace LehrplanGenerator.Logic.Services;

/// <summary>
/// Service zum Exportieren von Benutzer-Daten aus der Datenbank als JSON
/// </summary>
public class ExportService
{
    private readonly IChatRepository _chatRepository;
    private readonly AppState _appState;

    public ExportService(
        IChatRepository chatRepository,
        AppState appState)
    {
        _chatRepository = chatRepository;
        _appState = appState;
    }

    /// <summary>
    /// Exportiert alle Daten eines Benutzers als JSON
    /// </summary>
    public async Task<string> ExportUserDataAsJsonAsync(Guid userId)
    {
        var data = new
        {
            ExportDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
            UserId = userId,
            ChatSessions = await ExportChatSessionsAsync(userId),
            StudyPlans = "Exportierte Lernpläne",
            LearningProgress = "Exportierter Lernfortschritt"
        };

        var jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        return JsonConvert.SerializeObject(data, jsonSettings);
    }

    /// <summary>
    /// Speichert exportierte Daten in eine JSON-Datei
    /// </summary>
    public async Task<string> SaveExportToFileAsync(Guid userId, string? fileName = null)
    {
        var json = await ExportUserDataAsJsonAsync(userId);

        // Standard-Dateiname: "export_<datum>_<userid>.json"
        fileName ??= $"export_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}_{userId}.json";

        // Speichern im Benutzer-Documents-Ordner
        var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var filePath = Path.Combine(documentPath, "LehrplanGenerator", fileName);

        // Verzeichnis erstellen wenn nicht vorhanden
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        // Datei schreiben
        await File.WriteAllTextAsync(filePath, json);

        Console.WriteLine($"✓ Export gespeichert: {filePath}");
        return filePath;
    }

    /// <summary>
    /// Exportiert alle Chat-Sessions mit Nachrichten
    /// </summary>
    private async Task<object?> ExportChatSessionsAsync(Guid userId)
    {
        try
        {
            var sessions = await _chatRepository.GetUserSessionsAsync(userId);

            var sessionExports = new List<object>();
            foreach (var session in sessions)
            {
                var messages = await _chatRepository.GetSessionMessagesAsync(session.SessionId);

                sessionExports.Add(new
                {
                    SessionId = session.SessionId,
                    Title = session.Title,
                    Topic = session.Topic,
                    CreatedAt = session.CreatedAt,
                    UpdatedAt = session.UpdatedAt,
                    MessageCount = messages.Count(),
                    Messages = messages.Select(m => new
                    {
                        MessageId = m.MessageId,
                        Sender = m.Sender,
                        Text = m.Text,
                        CreatedAt = m.CreatedAt
                    }).ToList()
                });
            }

            return sessionExports;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Fehler beim Export von Chat-Sessions: {ex.Message}");
            return null;
        }
    }
}
