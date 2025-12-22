namespace LernApp.Models;

public class UserEinstellung
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Sprache { get; set; } = "de";
    public string Thema { get; set; } = "light";
    public bool BenachrichtigungenAktiv { get; set; } = true;
    public string? AIModell { get; set; } // z.B. "GPT-4", "Claude", etc.
    public DateTime AktualisiertAm { get; set; } = DateTime.Now;

    // Navigation Properties
    public User User { get; set; } = null!;
}
