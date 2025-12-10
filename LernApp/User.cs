using LernApp.Models;

public class User // Represents the User
{
    public int Id { get; set; }              // Primärschlüssel
    public string Name { get; set; } = "";   // Pflichtfeld
    public string Email { get; set; } = "";  // Eindeutige Email
    public string PasswordHash { get; set; } = ""; // Gehashtes Passwort
    public int LernzeitProTag { get; set; }  // Minuten pro Tag
    public DateTime ErstelltAm { get; set; } = DateTime.Now;
    public DateTime AktualisiertAm { get; set; } = DateTime.Now;

    // Navigation Properties
    public ICollection<LernEinheit> LernEinheiten { get; set; } = new List<LernEinheit>();
    public ICollection<Prompt> Prompts { get; set; } = new List<Prompt>();
    public ICollection<GenerierteCSV> GenerierteCSVs { get; set; } = new List<GenerierteCSV>();
    public UserEinstellung? Einstellungen { get; set; }
}
