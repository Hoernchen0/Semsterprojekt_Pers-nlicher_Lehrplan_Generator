using LernApp.Models;
using LernApp.Data.Repositories;

namespace LernApp.Services;

public interface IUserAppSettingsService
{
    Task<UserEinstellung> HoleEinstellungenAsync(int userId);
    Task<UserEinstellung> AktualisiereEinstellungenAsync(UserEinstellung einstellung);
    Task<UserEinstellung> ErstelleStandardEinstellungenAsync(int userId);
}

public class UserAppSettingsService : IUserAppSettingsService
{
    private readonly IRepository<UserEinstellung> _einstellungRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILernAppLogger _logger;

    public UserAppSettingsService(
        IRepository<UserEinstellung> einstellungRepository,
        IUserRepository userRepository,
        ILernAppLogger logger)
    {
        _einstellungRepository = einstellungRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserEinstellung> HoleEinstellungenAsync(int userId)
    {
        try
        {
            var einstellungen = await _einstellungRepository.FindAsync(e => e.UserId == userId);
            return einstellungen.FirstOrDefault() ?? await ErstelleStandardEinstellungenAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Fehler beim Abrufen der Einstellungen: {ex.Message}");
            throw;
        }
    }

    public async Task<UserEinstellung> AktualisiereEinstellungenAsync(UserEinstellung einstellung)
    {
        einstellung.AktualisiertAm = DateTime.Now;
        return await _einstellungRepository.UpdateAsync(einstellung);
    }

    public async Task<UserEinstellung> ErstelleStandardEinstellungenAsync(int userId)
    {
        try
        {
            var standardEinstellung = new UserEinstellung
            {
                UserId = userId,
                Sprache = "de",
                Thema = "light",
                BenachrichtigungenAktiv = true,
                AktualisiertAm = DateTime.Now
            };

            return await _einstellungRepository.AddAsync(standardEinstellung);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Fehler beim Erstellen der Standardeinstellungen: {ex.Message}");
            throw;
        }
    }
}
