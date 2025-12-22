namespace LernApp.Models;

public class GenerierteCSV
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? PromptId { get; set; }
    public string Dateiname { get; set; } = "";
    public string Inhalt { get; set; } = "";
    public DateTime ErstelltAm { get; set; } = DateTime.Now;
    public string? Beschreibung { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;
    public Prompt? Prompt { get; set; }
}
