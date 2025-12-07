namespace LernApp.Models;

public class LernUnterlage
{
    public int Id { get; set; }

    public string Titel { get; set; } = "";

    // hier speichern wir das XML als Text
    public string XmlInhalt { get; set; } = "";

    // optional Metadaten
    public DateTime HochgeladenAm { get; set; } = DateTime.Now;
}
