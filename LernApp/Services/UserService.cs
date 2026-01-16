using LernApp.Models;
using LernApp.Data.Repositories;

namespace LernApp.Services;

public interface IUserService
{
    Task<User> RegisteriereBenutzerAsync(string name, string email, string passwordHash);
    Task<User?> AuthentifiziereBenutzerAsync(string email, string passwordHash);
    Task<User?> HoleBenutzerAsync(int userId);
    Task<User?> HoleBenutzerAsync(string email); // Overload for email lookup
    Task<User> AktualisiereBenutzerAsync(User user);
    Task<bool> ExistiertEmailAsync(string email);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILernAppLogger _logger;

    public UserService(IUserRepository userRepository, ILernAppLogger logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<User> RegisteriereBenutzerAsync(string name, string email, string passwordHash)
    {
        try
        {
            if (await ExistiertEmailAsync(email))
            {
                throw new InvalidOperationException($"Email {email} existiert bereits");
            }

            var user = new User
            {
                Name = name,
                Email = email,
                PasswordHash = passwordHash,
                ErstelltAm = DateTime.Now,
                AktualisiertAm = DateTime.Now
            };

            var erstellterBenutzer = await _userRepository.AddAsync(user);
            _logger.LogInfo($"Benutzer registriert: {erstellterBenutzer.Email}");
            return erstellterBenutzer;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Fehler bei der Registrierung: {ex.Message}");
            throw;
        }
    }

    public async Task<User?> AuthentifiziereBenutzerAsync(string email, string passwordHash)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user != null && user.PasswordHash == passwordHash)
            {
                _logger.LogInfo($"Benutzer authentifiziert: {email}");
                return user;
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Fehler bei der Authentifizierung: {ex.Message}");
            throw;
        }
    }

    public async Task<User?> HoleBenutzerAsync(int userId)
    {
        return await _userRepository.GetByIdAsync(userId);
    }

    public async Task<User?> HoleBenutzerAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public async Task<User> AktualisiereBenutzerAsync(User user)
    {
        user.AktualisiertAm = DateTime.Now;
        return await _userRepository.UpdateAsync(user);
    }

    public async Task<bool> ExistiertEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user != null;
    }
}
