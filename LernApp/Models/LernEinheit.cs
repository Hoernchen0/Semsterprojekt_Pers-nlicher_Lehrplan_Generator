using SQLite;

namespace LernApp.Models;

public class LernEinheit
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Fach { get; set; } = "";
    public string Thema { get; set; } = "";
    public DateTime Datum { get; set; }
}
