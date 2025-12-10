using LernApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LernApp.Data.Repositories;

public interface IGenerierteCSVRepository : IRepository<GenerierteCSV>
{
    Task<IEnumerable<GenerierteCSV>> GetByUserIdAsync(int userId);
    Task<IEnumerable<GenerierteCSV>> GetByPromptIdAsync(int promptId);
}

public class GenerierteCSVRepository : Repository<GenerierteCSV>, IGenerierteCSVRepository
{
    private readonly LernAppDbContext _context;

    public GenerierteCSVRepository(LernAppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<GenerierteCSV>> GetByUserIdAsync(int userId)
    {
        return await _context.GenerierteCSVs
            .Where(g => g.UserId == userId)
            .OrderByDescending(g => g.ErstelltAm)
            .ToListAsync();
    }

    public async Task<IEnumerable<GenerierteCSV>> GetByPromptIdAsync(int promptId)
    {
        return await _context.GenerierteCSVs
            .Where(g => g.PromptId == promptId)
            .ToListAsync();
    }
}
