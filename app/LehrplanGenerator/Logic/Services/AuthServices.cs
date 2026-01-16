using System;
using System.Threading.Tasks;
using LehrplanGenerator.Logic.Utils;
using LehrplanGenerator.Logic.Models;
using LernApp.Services;
using UserAlias = LehrplanGenerator.Logic.Models.User;

namespace LehrplanGenerator.Logic.Services;

public class AuthService
{
    private readonly IUserService _userService;

    public UserAlias? CurrentUser { get; private set; }

    public AuthService(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<bool> RegisterAsync(string firstName, string lastName, string email, string password)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userService.HoleBenutzerAsync(email);
            if (existingUser != null)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Registrierung: E-Mail {email} existiert bereits");
                return false;
            }

            // Register new user with LernApp service (uses SQLite)
            var hashedPassword = PasswordHasher.Hash(password);
            var user = await _userService.RegisteriereBenutzerAsync(
                name: $"{firstName} {lastName}",
                email: email,
                passwordHash: hashedPassword
            );

            CurrentUser = new UserAlias(Guid.NewGuid(), firstName, lastName);
            System.Diagnostics.Debug.WriteLine($"✅ Benutzer registriert: {email} (ID: {user.Id})");
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Registrierungsfehler: {ex.Message}\n{ex.StackTrace}");
            return false;
        }
    }

    public async Task<UserAlias?> LoginAsync(string email, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                System.Diagnostics.Debug.WriteLine($"❌ Login: E-Mail oder Passwort leer");
                return null;
            }

            // Authenticate using LernApp service (uses SQLite)
            var hashedPassword = PasswordHasher.Hash(password);
            System.Diagnostics.Debug.WriteLine($"🔐 Login-Versuch für: {email}");
            
            var user = await _userService.AuthentifiziereBenutzerAsync(email, hashedPassword);
            if (user == null)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Login fehlgeschlagen: Benutzer oder Passwort falsch ({email})");
                return null;
            }

            // Parse name into firstName/lastName
            var names = user.Name.Split(' ', 2);
            var firstName = names[0];
            var lastName = names.Length > 1 ? names[1] : "";

            CurrentUser = new UserAlias(Guid.NewGuid(), firstName, lastName);
            System.Diagnostics.Debug.WriteLine($"✅ Login erfolgreich: {email}");
            return CurrentUser;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Login-Fehler: {ex.Message}\n{ex.StackTrace}");
            return null;
        }
    }
}

