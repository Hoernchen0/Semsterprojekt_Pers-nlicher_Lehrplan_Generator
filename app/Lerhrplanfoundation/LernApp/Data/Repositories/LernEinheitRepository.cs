using LernApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LernApp.Data.Repositories;

public interface ILernEinheitRepository : IRepository<LernEinheit>
{
    Task<IEnumerable<LernEinheit>> GetByUserIdAsync(int userId);
    Task<LernEinheit?> GetWithAnalysenAsync(int id);
    Task<IEnumerable<LernEinheit>> GetByFachAsync(string fach, int userId);
}

public class LernEinheitRepository : Repository<LernEinheit>, ILernEinheitRepository
{
    private readonly LernAppDbContext _context;

    public LernEinheitRepository(LernAppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LernEinheit>> GetByUserIdAsync(int userId)
    {
        return await _context.LernEinheiten
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.Datum)
            .ToListAsync();
    }

    public async Task<LernEinheit?> GetWithAnalysenAsync(int id)
    {
        return await _context.LernEinheiten
            .Include(l => l.DateiAnalysen)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<IEnumerable<LernEinheit>> GetByFachAsync(string fach, int userId)
    {
        return await _context.LernEinheiten
            .Where(l => l.UserId == userId && l.Fach == fach)
            .ToListAsync();
    }
}
