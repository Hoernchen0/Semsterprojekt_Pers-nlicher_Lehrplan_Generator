using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using LehrplanGenerator.Logic.Models;

namespace LehrplanGenerator.Logic.Services;

public class UserCredentialStore
{
    private const string FilePath = "datenbank.json";
    private List<UserCredential> _cache = new();

    public UserCredentialStore()
    {
        Load();
    }

    private void Load()
    {
        if (!File.Exists(FilePath))
        {
            _cache = new();
            return;
        }

        var json = File.ReadAllText(FilePath);
        _cache = JsonSerializer.Deserialize<List<UserCredential>>(json)
                 ?? new List<UserCredential>();
    }

    private void Save()
    {
        var json = JsonSerializer.Serialize(_cache, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(FilePath, json);
    }

    public bool UsernameExists(string username)
        => _cache.Any(c => c.Username == username);

    public void Add(UserCredential cred)
    {
        _cache.Add(cred);
        Save();
    }

    public IReadOnlyList<UserCredential> GetAll()
    {
        return _cache;
    }
}
