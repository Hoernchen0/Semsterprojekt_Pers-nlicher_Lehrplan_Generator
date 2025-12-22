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
        _cache = JsonSerializer.Deserialize<List<UserCredential>>(json) ?? new();
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

    public void Add(UserCredential user)
    {
        _cache.Add(user);
        Save();
    }

    public UserCredential? GetByUsername(string username)
        => _cache.FirstOrDefault(u => u.Username == username);

    public IReadOnlyList<UserCredential> GetAll() => _cache;
}
