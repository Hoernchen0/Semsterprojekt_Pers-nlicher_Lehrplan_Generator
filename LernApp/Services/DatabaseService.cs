using SQLite;
using LernApp.Models;

namespace LernApp.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _db;

    public DatabaseService()
    {
        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "lernplan.db");
        _db = new SQLiteAsyncConnection(dbPath);
        _db.CreateTableAsync<LernEinheit>().Wait();
    }

    public Task<List<LernEinheit>> LadeAlleAsync() => _db.Table<LernEinheit>().ToListAsync();
    public Task SpeichernAsync(LernEinheit einheit) => _db.InsertAsync(einheit);
}
