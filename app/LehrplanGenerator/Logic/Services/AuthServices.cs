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
                return false;

            // Register new user with LernApp service (uses SQLite)
            var user = await _userService.RegisteriereBenutzerAsync(
                name: $"{firstName} {lastName}",
                email: email,
                passwordHash: PasswordHasher.Hash(password)
            );

            CurrentUser = new UserAlias(Guid.NewGuid(), firstName, lastName);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<UserAlias?> LoginAsync(string email, string password)
    {
        try
        {
            // Authenticate using LernApp service (uses SQLite)
            var user = await _userService.AuthentifiziereBenutzerAsync(email, PasswordHasher.Hash(password));
            if (user == null)
                return null;

            // Parse name into firstName/lastName
            var names = user.Name.Split(' ', 2);
            var firstName = names[0];
            var lastName = names.Length > 1 ? names[1] : "";

            CurrentUser = new UserAlias(Guid.NewGuid(), firstName, lastName);
            return CurrentUser;
        }
        catch
        {
            return null;
        }
    }
}

