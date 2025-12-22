using LernApp.Models;
using LernApp.Data.Repositories;

namespace LernApp.Services;

public interface IAIService
{
    Task<string> GeneriereLernplanAsync(string userInput, int userId);
    Task<Prompt> SpeicherePromptAsync(int userId, string text, string response, string? kategorie = null);
    Task<IEnumerable<Prompt>> HolePromptsAsync(int userId);
    Task<IEnumerable<Prompt>> HolePromptsNachKategorieAsync(string kategorie, int userId);
    Task RufeAIPythonScriptAsync(string prompt);
}

public class AIService : IAIService
{
    private readonly IPromptRepository _promptRepository;
    private readonly ILernAppLogger _logger;
    // TODO: Python Script Kommunikation hinzufügen

    public AIService(IPromptRepository promptRepository, ILernAppLogger logger)
    {
        _promptRepository = promptRepository;
        _logger = logger;
    }

    public async Task<string> GeneriereLernplanAsync(string userInput, int userId)
    {
        try
        {
            _logger.LogInfo($"Generiere Lernplan für Benutzer {userId}");
            
            // TODO: Python Script aufrufen
            string aiResponse = ""; // await RufeAIPythonScriptAsync(userInput);
            
            // Speichere den Prompt
            await SpeicherePromptAsync(userId, userInput, aiResponse, "Lernplan");
            
            return aiResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Fehler bei der KI-Generierung: {ex.Message}");
            throw;
        }
    }

    public async Task<Prompt> SpeicherePromptAsync(int userId, string text, string response, string? kategorie = null)
    {
        var prompt = new Prompt
        {
            UserId = userId,
            Text = text,
            Response = response,
            Kategorie = kategorie,
            ErstelltAm = DateTime.Now
        };

        return await _promptRepository.AddAsync(prompt);
    }

    public async Task<IEnumerable<Prompt>> HolePromptsAsync(int userId)
    {
        return await _promptRepository.GetByUserIdAsync(userId);
    }

    public async Task<IEnumerable<Prompt>> HolePromptsNachKategorieAsync(string kategorie, int userId)
    {
        return await _promptRepository.GetByKategorieAsync(kategorie, userId);
    }

    public async Task RufeAIPythonScriptAsync(string prompt)
    {
        try
        {
            // TODO: Implementierung für Python Script Kommunikation
            // Beispiel:
            // using (var process = new Process())
            // {
            //     process.StartInfo.FileName = "python3";
            //     process.StartInfo.Arguments = $"ai_script.py \"{prompt}\"";
            //     process.StartInfo.UseShellExecute = false;
            //     process.StartInfo.RedirectStandardOutput = true;
            //     process.Start();
            //     string output = await process.StandardOutput.ReadToEndAsync();
            //     return output;
            // }
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Fehler beim Aufruf des Python Scripts: {ex.Message}");
            throw;
        }
    }
}
