using Microsoft.EntityFrameworkCore;
using LernApp.Models;

namespace LernApp.Data;

public class LernAppDbContext : DbContext
{
    public LernAppDbContext(DbContextOptions<LernAppDbContext> options) : base(options)
    {
    }

    // DbSets für alle Entities
    public DbSet<User> Users { get; set; }
    public DbSet<LernEinheit> LernEinheiten { get; set; }
    public DbSet<Prompt> Prompts { get; set; }
    public DbSet<GenerierteCSV> GenerierteCSVs { get; set; }
    public DbSet<DateiAnalyse> DateiAnalysen { get; set; }
    public DbSet<UserEinstellung> UserEinstellungen { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Konfiguration
        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);
        modelBuilder.Entity<User>()
            .HasMany(u => u.LernEinheiten)
            .WithOne(l => l.User)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<User>()
            .HasMany(u => u.Prompts)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<User>()
            .HasMany(u => u.GenerierteCSVs)
            .WithOne(g => g.User)
            .HasForeignKey(g => g.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // LernEinheit Konfiguration
        modelBuilder.Entity<LernEinheit>()
            .HasKey(l => l.Id);
        modelBuilder.Entity<LernEinheit>()
            .HasMany(l => l.DateiAnalysen)
            .WithOne(d => d.LernEinheit)
            .HasForeignKey(d => d.LernEinheitId)
            .OnDelete(DeleteBehavior.Cascade);

        // Prompt Konfiguration
        modelBuilder.Entity<Prompt>()
            .HasKey(p => p.Id);
        modelBuilder.Entity<Prompt>()
            .HasOne(p => p.GenerierteCSV)
            .WithOne(g => g.Prompt)
            .HasForeignKey<GenerierteCSV>(g => g.PromptId)
            .OnDelete(DeleteBehavior.SetNull);

        // GenerierteCSV Konfiguration
        modelBuilder.Entity<GenerierteCSV>()
            .HasKey(g => g.Id);

        // DateiAnalyse Konfiguration
        modelBuilder.Entity<DateiAnalyse>()
            .HasKey(d => d.Id);

        // UserEinstellung Konfiguration
        modelBuilder.Entity<UserEinstellung>()
            .HasKey(u => u.Id);
    }
}
