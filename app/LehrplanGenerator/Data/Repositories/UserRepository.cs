using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LehrplanGenerator.Logic.Models;

namespace LehrplanGenerator.Data.Repositories;

public interface IUserRepository
{
    Task<UserCredentialEntity?> GetByUsernameAsync(string username);
    Task<UserCredentialEntity?> GetByUserIdAsync(Guid userId);
    Task<UserCredentialEntity> AddAsync(UserCredentialEntity user);
    Task<UserCredentialEntity> UpdateAsync(UserCredentialEntity user);
    Task<bool> UsernameExistsAsync(string username);
}

public class UserRepository : IUserRepository
{
    private readonly LehrplanDbContext _context;

    public UserRepository(LehrplanDbContext context)
    {
        _context = context;
    }

    public async Task<UserCredentialEntity?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<UserCredentialEntity?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<UserCredentialEntity> AddAsync(UserCredentialEntity user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<UserCredentialEntity> UpdateAsync(UserCredentialEntity user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _context.Users
            .AnyAsync(u => u.Username == username);
    }
}
