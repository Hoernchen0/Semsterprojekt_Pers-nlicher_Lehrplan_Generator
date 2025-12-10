namespace LernApp.Models;

public class Prompt
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Text { get; set; } = "";
    public string Response { get; set; } = "";
    public DateTime ErstelltAm { get; set; } = DateTime.Now;
    public string? Kategorie { get; set; } // z.B. "Lernplan", "Zusammenfassung", etc.

    // Navigation Properties
    public User User { get; set; } = null!;
    public GenerierteCSV? GenerierteCSV { get; set; }
}
