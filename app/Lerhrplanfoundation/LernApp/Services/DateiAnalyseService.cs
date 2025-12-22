using LernApp.Models;
using LernApp.Data.Repositories;

namespace LernApp.Services;

public interface IDateiAnalyseService
{
    Task<DateiAnalyse> AnalysiereDateiAsync(int lernEinheitId, string dateiname, string inhalt);
    Task<DateiAnalyse?> HoleDateiAnalyseAsync(int id);
    Task<IEnumerable<DateiAnalyse>> HoleDateiAnalysenAsync(int lernEinheitId);
}

public class DateiAnalyseService : IDateiAnalyseService
{
    private readonly IRepository<DateiAnalyse> _dateiAnalyseRepository;
    private readonly ILernEinheitRepository _lernEinheitRepository;
    private readonly ILernAppLogger _logger;

    public DateiAnalyseService(
        IRepository<DateiAnalyse> dateiAnalyseRepository,
        ILernEinheitRepository lernEinheitRepository,
        ILernAppLogger logger)
    {
        _dateiAnalyseRepository = dateiAnalyseRepository;
        _lernEinheitRepository = lernEinheitRepository;
        _logger = logger;
    }

    public async Task<DateiAnalyse> AnalysiereDateiAsync(int lernEinheitId, string dateiname, string inhalt)
    {
        try
        {
            // Verifiziere, dass die Lerneinheit existiert
            var lernEinheit = await _lernEinheitRepository.GetByIdAsync(lernEinheitId);
            if (lernEinheit == null)
            {
                throw new ArgumentException($"Lerneinheit {lernEinheitId} nicht gefunden");
            }

            // TODO: Dateiinhalt analysieren (mit KI oder anderen Methoden)
            var zusammenfassung = $"Zusammenfassung von {dateiname}";

            var analyse = new DateiAnalyse
            {
                LernEinheitId = lernEinheitId,
                Dateiname = dateiname,
                InhaltZusammenfassung = zusammenfassung,
                AnalysiertAm = DateTime.Now
            };

            var erstellteAnalyse = await _dateiAnalyseRepository.AddAsync(analyse);
            _logger.LogInfo($"Datei analysiert: {dateiname}");
            return erstellteAnalyse;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Fehler bei der Dateianalyse: {ex.Message}");
            throw;
        }
    }

    public async Task<DateiAnalyse?> HoleDateiAnalyseAsync(int id)
    {
        return await _dateiAnalyseRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<DateiAnalyse>> HoleDateiAnalysenAsync(int lernEinheitId)
    {
        return await _dateiAnalyseRepository.FindAsync(d => d.LernEinheitId == lernEinheitId);
    }
}
