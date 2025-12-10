using LernApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LernApp.Data.Repositories;

public interface IPromptRepository : IRepository<Prompt>
{
    Task<IEnumerable<Prompt>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Prompt>> GetByKategorieAsync(string kategorie, int userId);
}

public class PromptRepository : Repository<Prompt>, IPromptRepository
{
    private readonly LernAppDbContext _context;

    public PromptRepository(LernAppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Prompt>> GetByUserIdAsync(int userId)
    {
        return await _context.Prompts
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.ErstelltAm)
            .ToListAsync();
    }

    public async Task<IEnumerable<Prompt>> GetByKategorieAsync(string kategorie, int userId)
    {
        return await _context.Prompts
            .Where(p => p.UserId == userId && p.Kategorie == kategorie)
            .ToListAsync();
    }
}
