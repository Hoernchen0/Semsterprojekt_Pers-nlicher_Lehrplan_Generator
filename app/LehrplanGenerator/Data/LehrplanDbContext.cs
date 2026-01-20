using Microsoft.EntityFrameworkCore;
using LehrplanGenerator.Logic.Models;

namespace LehrplanGenerator.Data;

public class LehrplanDbContext : DbContext
{
    public LehrplanDbContext(DbContextOptions<LehrplanDbContext> options)
        : base(options)
    {
    }

    // =========================
    // DbSets
    // =========================
    public DbSet<UserCredentialEntity> Users { get; set; } = null!;
    public DbSet<ChatSessionEntity> ChatSessions { get; set; } = null!;
    public DbSet<ChatMessageEntity> ChatMessages { get; set; } = null!;
    public DbSet<StudyPlanEntity> StudyPlans { get; set; } = null!;
    public DbSet<LearningProgressEntity> LearningProgress { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =========================
        // User
        // =========================
        modelBuilder.Entity<UserCredentialEntity>()
            .HasKey(u => u.UserId);

        modelBuilder.Entity<UserCredentialEntity>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<UserCredentialEntity>()
            .HasMany(u => u.ChatSessions)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserCredentialEntity>()
            .HasMany(u => u.ChatMessages)
            .WithOne(m => m.User)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserCredentialEntity>()
            .HasMany(u => u.StudyPlans)
            .WithOne(sp => sp.User)
            .HasForeignKey(sp => sp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // =========================
        // StudyPlan
        // =========================
        modelBuilder.Entity<StudyPlanEntity>()
            .HasKey(sp => sp.Id);

        modelBuilder.Entity<StudyPlanEntity>()
            .Property(sp => sp.Title)
            .IsRequired()
            .HasMaxLength(200);

        modelBuilder.Entity<StudyPlanEntity>()
            .HasMany(sp => sp.LearningUnits)
            .WithOne(lp => lp.StudyPlan)
            .HasForeignKey(lp => lp.StudyPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        // =========================
        // ChatSession
        // =========================
        modelBuilder.Entity<ChatSessionEntity>()
            .HasKey(s => s.SessionId);

        modelBuilder.Entity<ChatSessionEntity>()
            .HasIndex(s => new { s.UserId, s.CreatedAt });

        modelBuilder.Entity<ChatSessionEntity>()
            .HasMany(s => s.Messages)
            .WithOne(m => m.Session)
            .HasForeignKey(m => m.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        // =========================
        // ChatMessage
        // =========================
        modelBuilder.Entity<ChatMessageEntity>()
            .HasKey(m => m.MessageId);

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


        // =========================
        // LearningProgress
        // =========================
        modelBuilder.Entity<LearningProgressEntity>()
            .HasKey(lp => lp.Id);

        modelBuilder.Entity<LearningProgressEntity>()
            .Property(lp => lp.Subject)
            .IsRequired();

        modelBuilder.Entity<LearningProgressEntity>()
            .HasIndex(lp => new { lp.UserId, lp.StudyPlanId, lp.PlannedStart });
    }
}
