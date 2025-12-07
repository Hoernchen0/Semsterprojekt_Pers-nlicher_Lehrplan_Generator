using System;
using SQLite;
using System.IO;
using System.Threading.Tasks;

public class LernEinheit
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Fach { get; set; } = "";
    public string Thema { get; set; } = "";
    public DateTime Datum { get; set; }
}

public class User
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
}

class DbSetup
{
    static async Task Main(string[] args)
    {
        string dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "lernapp.db"
        );

        Console.WriteLine($"Datenbank wird erstellt: {dbPath}");

        var db = new SQLiteAsyncConnection(dbPath);

        await db.CreateTableAsync<User>();
        await db.CreateTableAsync<LernEinheit>();

        Console.WriteLine("Tabellen erfolgreich erstellt!");
    }
}
