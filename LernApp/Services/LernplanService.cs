using LernApp.Models;
using LernApp.Data.Repositories;

namespace LernApp.Services;

public interface ILernplanService
{
    Task<LernEinheit> ErstelleLernEinheitAsync(int userId, string fach, string thema, string? beschreibung = null);
    Task<IEnumerable<LernEinheit>> HoleLernEinheitenAsync(int userId);
    Task<LernEinheit?> HoleLernEinheitAsync(int id);
    Task<IEnumerable<LernEinheit>> HoleLernEinheitenNachFachAsync(string fach, int userId);
    Task<LernEinheit> AktualisiereLernEinheitAsync(LernEinheit lernEinheit);
    Task LöscheLernEinheitAsync(int id);
}

public class LernplanService : ILernplanService
{
    private readonly ILernEinheitRepository _lernEinheitRepository;
    private readonly ILernAppLogger _logger;

    public LernplanService(ILernEinheitRepository lernEinheitRepository, ILernAppLogger logger)
    {
        _lernEinheitRepository = lernEinheitRepository;
        _logger = logger;
    }

    public async Task<LernEinheit> ErstelleLernEinheitAsync(int userId, string fach, string thema, string? beschreibung = null)
    {
        try
        {
            var lernEinheit = new LernEinheit
            {
                UserId = userId,
                Fach = fach,
                Thema = thema,
                Beschreibung = beschreibung,
                Datum = DateTime.Now
            };

            var erstellteEinheit = await _lernEinheitRepository.AddAsync(lernEinheit);
            _logger.LogInfo($"Lerneinheit erstellt: {erstellteEinheit.Id} für Benutzer {userId}");
            return erstellteEinheit;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Fehler beim Erstellen der Lerneinheit: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<LernEinheit>> HoleLernEinheitenAsync(int userId)
    {
        try
        {
            return await _lernEinheitRepository.GetByUserIdAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Fehler beim Abrufen der Lerneinheiten: {ex.Message}");
            throw;
        }
    }

    public async Task<LernEinheit?> HoleLernEinheitAsync(int id)
    {
        return await _lernEinheitRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<LernEinheit>> HoleLernEinheitenNachFachAsync(string fach, int userId)
    {
        return await _lernEinheitRepository.GetByFachAsync(fach, userId);
    }

    public async Task<LernEinheit> AktualisiereLernEinheitAsync(LernEinheit lernEinheit)
    {
        lernEinheit.AktualisiertAm = DateTime.Now;
        return await _lernEinheitRepository.UpdateAsync(lernEinheit);
    }

    public async Task LöscheLernEinheitAsync(int id)
    {
        await _lernEinheitRepository.DeleteAsync(id);
        _logger.LogInfo($"Lerneinheit {id} gelöscht");
    }
}
