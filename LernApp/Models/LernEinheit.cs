namespace LernApp.Models;

public class LernEinheit
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Fach { get; set; } = "";
    public string Thema { get; set; } = "";
    public string? Beschreibung { get; set; }
    public DateTime Datum { get; set; } = DateTime.Now;
    public DateTime ErstelltAm { get; set; } = DateTime.Now;
    public DateTime AktualisiertAm { get; set; } = DateTime.Now;

    // Navigation Properties
    public User User { get; set; } = null!;
    public ICollection<DateiAnalyse> DateiAnalysen { get; set; } = new List<DateiAnalyse>();
}
