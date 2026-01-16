using System;
using System.Threading.Tasks;
using LehrplanGenerator.Logic.Utils;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Data.Repositories;

namespace LehrplanGenerator.Logic.Services;

/// <summary>
/// Enhanced AuthService mit Datenbank-Integration
/// Speichert Benutzer in SQLite und authentifiziert sie
/// </summary>
public class AuthServiceDb
{
    private readonly IUserRepository _userRepository;

    public AuthServiceDb(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Registriert einen neuen Benutzer in der Datenbank
    /// </summary>
    public async Task<(bool Success, string Message)> RegisterAsync(
        string firstName, 
        string lastName, 
        string username, 
        string password,
        string? email = null)
    {
        // Check if username already exists
        if (await _userRepository.UsernameExistsAsync(username))
            return (false, "Benutzername existiert bereits");

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            return (false, "Passwort muss mindestens 6 Zeichen lang sein");

        try
        {
            var passwordHash = PasswordHasher.Hash(password);
            var user = new UserCredentialEntity
            {
                UserId = Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                Username = username,
                PasswordHash = passwordHash,
                Email = email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            return (true, "Registrierung erfolgreich");
        }
        catch (Exception ex)
        {
            return (false, $"Registrierungsfehler: {ex.Message}");
        }
    }

    /// <summary>
    /// Authentifiziert einen Benutzer mit Benutzername und Passwort
    /// </summary>
    public async Task<(bool Success, UserCredentialEntity? User, string Message)> LoginAsync(
        string username, 
        string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return (false, null, "Benutzername und Passwort erforderlich");

        try
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
                return (false, null, "Benutzer nicht gefunden");

            var passwordHash = PasswordHasher.Hash(password);
            if (passwordHash != user.PasswordHash)
                return (false, null, "Passwort ungültig");

            return (true, user, "Anmeldung erfolgreich");
        }
        catch (Exception ex)
        {
            return (false, null, $"Anmeldungsfehler: {ex.Message}");
        }
    }

    /// <summary>
    /// Holt einen Benutzer nach seiner ID
    /// </summary>
    public async Task<UserCredentialEntity?> GetUserByIdAsync(Guid userId)
    {
        return await _userRepository.GetByUserIdAsync(userId);
    }

    /// <summary>
    /// Holt einen Benutzer nach seinem Benutzernamen
    /// </summary>
    public async Task<UserCredentialEntity?> GetUserByUsernameAsync(string username)
    {
        return await _userRepository.GetByUsernameAsync(username);
    }
}
