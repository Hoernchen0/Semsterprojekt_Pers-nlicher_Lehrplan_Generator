namespace LernApp.Models;

public class LernEinheit
{
    public int Id { get; set; }
    public string Fach { get; set; } = "";
    public string Thema { get; set; } = "";
    public DateTime Datum { get; set; }
}
