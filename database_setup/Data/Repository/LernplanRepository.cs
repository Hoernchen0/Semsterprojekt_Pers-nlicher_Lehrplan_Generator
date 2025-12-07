using Microsoft.EntityFrameworkCore;
using LernApp.Models;

namespace LernApp.Data.Repository;

public class LernUnterlagenRepository
{
    private readonly LernAppDbContext _db = new LernAppDbContext();

    public Task<List<LernUnterlage>> GetAllAsync()
        => _db.LernUnterlagen.ToListAsync();

    public async Task AddAsync(LernUnterlage u)
    {
        _db.LernUnterlagen.Add(u);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(LernUnterlage u)
    {
        _db.LernUnterlagen.Update(u);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _db.LernUnterlagen.FindAsync(id);
        if (entity != null)
        {
            _db.LernUnterlagen.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
