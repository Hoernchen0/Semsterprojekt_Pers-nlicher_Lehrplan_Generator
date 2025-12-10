using LernApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LernApp.Data.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetWithEinstellungenAsync(int userId);
    Task<User?> GetWithLernEinheitenAsync(int userId);
}

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly LernAppDbContext _context;

    public UserRepository(LernAppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetWithEinstellungenAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.Einstellungen)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<User?> GetWithLernEinheitenAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.LernEinheiten)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
}
