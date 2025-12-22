using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using LehrplanGenerator.Logic.Models;
using UserAlias = LehrplanGenerator.Logic.Models.User;

namespace LehrplanGenerator.Logic.Services;

public class AuthService
{
    private readonly string _file;

    public UserAlias? CurrentUser { get; private set; }

    public AuthService()
    {
        _file = Path.Combine(AppContext.BaseDirectory, "login.json");
    }

    public bool Login(string username, string password)
    {
        if (!File.Exists(_file))
            return false;

        var json = File.ReadAllText(_file);
        var saved = JsonSerializer.Deserialize<LoginData>(json);

        if (saved == null)
            return false;

        if (saved.Username == username && saved.PasswordHash == Hash(password))
        {
            // Benutzer laden → User erzeugen
            CurrentUser = new UserAlias(
                saved.UserId,
                saved.Username
            );

            return true;
        }

        return false;
    }

    public void Register(string username, string password)
    {
        var data = new LoginData
        {
            UserId = Guid.NewGuid(),
            Username = username,
            PasswordHash = Hash(password)
        };

        File.WriteAllText(_file, JsonSerializer.Serialize(data));
    }

    private string Hash(string s)
    {
        using var sha = SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(s)));
    }

    private class LoginData
    {
        public Guid UserId { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }

        public LoginData() { }
    }
}
