using Microsoft.EntityFrameworkCore;
using LehrplanGenerator.Logic.Models;

namespace LehrplanGenerator.Data;

public class LehrplanDbContext : DbContext
{
    public LehrplanDbContext(DbContextOptions<LehrplanDbContext> options) : base(options)
    {
    }

    public DbSet<UserCredentialEntity> Users { get; set; } = null!;
    public DbSet<ChatSessionEntity> ChatSessions { get; set; } = null!;
    public DbSet<ChatMessageEntity> ChatMessages { get; set; } = null!;
    public DbSet<DayPlanEntity> DayPlans { get; set; } = null!;
    public DbSet<TaskItemEntity> TaskItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Entity
        modelBuilder.Entity<UserCredentialEntity>()
            .HasKey(u => u.UserId);

        modelBuilder.Entity<UserCredentialEntity>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<UserCredentialEntity>()
            .Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<UserCredentialEntity>()
            .Property(u => u.FirstName)
            .HasMaxLength(100);

        modelBuilder.Entity<UserCredentialEntity>()
            .Property(u => u.LastName)
            .HasMaxLength(100);

        modelBuilder.Entity<UserCredentialEntity>()
            .Property(u => u.Email)
            .HasMaxLength(255);

        // User -> ChatSessions: One-to-Many
        modelBuilder.Entity<UserCredentialEntity>()
            .HasMany(u => u.ChatSessions)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // User -> ChatMessages: One-to-Many
        modelBuilder.Entity<UserCredentialEntity>()
            .HasMany(u => u.ChatMessages)
            .WithOne(m => m.User)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ChatSession Entity
        modelBuilder.Entity<ChatSessionEntity>()
            .HasKey(s => s.SessionId);

        // ChatMessage Entity - PRIMARY KEY DEFINIEREN!
        modelBuilder.Entity<ChatMessageEntity>()
            .HasKey(m => m.MessageId);

        // ChatSession -> ChatMessages: One-to-Many
        modelBuilder.Entity<ChatSessionEntity>()
            .HasMany(s => s.Messages)
            .WithOne(m => m.Session)
            .HasForeignKey(m => m.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        modelBuilder.Entity<ChatMessageEntity>()
            .HasIndex(m => new { m.SessionId, m.CreatedAt });

        modelBuilder.Entity<ChatSessionEntity>()
            .HasIndex(s => new { s.UserId, s.CreatedAt });

        // ===== KALENDER / DAYPLAN KONFIGURATION =====

        // DayPlan Entity - PRIMARY KEY
        modelBuilder.Entity<DayPlanEntity>()
            .HasKey(d => d.DayPlanId);

        // TaskItem Entity - PRIMARY KEY
        modelBuilder.Entity<TaskItemEntity>()
            .HasKey(t => t.TaskId);

        // User -> DayPlans: One-to-Many
        modelBuilder.Entity<UserCredentialEntity>()
            .HasMany<DayPlanEntity>()
            .WithOne(d => d.User)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // DayPlan -> TaskItems: One-to-Many
        modelBuilder.Entity<DayPlanEntity>()
            .HasMany(d => d.Tasks)
            .WithOne(t => t.DayPlan)
            .HasForeignKey(t => t.DayPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        // TaskItem Indizes für Performance
        modelBuilder.Entity<TaskItemEntity>()
            .HasIndex(t => new { t.DayPlanId, t.CreatedAt });

        modelBuilder.Entity<DayPlanEntity>()
            .HasIndex(d => new { d.UserId, d.Day })
            .IsUnique(); // Ein Tag pro Nutzer
    }
}
