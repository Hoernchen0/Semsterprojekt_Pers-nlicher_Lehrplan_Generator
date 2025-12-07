using Microsoft.EntityFrameworkCore;
using LernApp.Models;

namespace LernApp.Data;

public class LernAppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<LernEinheit> LernEinheiten => Set<LernEinheit>();
    public DbSet<LernUnterlage> LernUnterlagen => Set<LernUnterlage>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "lernapp.db");

        options.UseSqlite($"Data Source={path}");
    }
}
