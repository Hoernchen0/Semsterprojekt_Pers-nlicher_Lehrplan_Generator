namespace LernApp.Models;

public class DateiAnalyse
{
    public int Id { get; set; }
    public int LernEinheitId { get; set; }
    public string Dateiname { get; set; } = "";
    public string InhaltZusammenfassung { get; set; } = "";
    public DateTime AnalysiertAm { get; set; } = DateTime.Now;
    public string? DateityP { get; set; } // z.B. "PDF", "DOCX", "TXT"

    // Navigation Properties
    public LernEinheit LernEinheit { get; set; } = null!;
}
