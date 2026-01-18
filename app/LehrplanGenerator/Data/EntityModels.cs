using System;
using System.Collections.Generic;
using LehrplanGenerator.Models.Chat;

namespace LehrplanGenerator.Logic.Models;

// ================= USER =================
public class UserCredentialEntity
{
    public Guid UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    // IST IN DER DB → bleibt
    public string? Email { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<ChatSessionEntity> ChatSessions { get; set; } = new List<ChatSessionEntity>();
    public ICollection<ChatMessageEntity> ChatMessages { get; set; } = new List<ChatMessageEntity>();
    public ICollection<StudyPlanEntity> StudyPlans { get; set; } = new List<StudyPlanEntity>();

    public string DisplayName => $"{FirstName} {LastName}";

    // Mapping
    public static UserCredentialEntity FromCredential(UserCredential cred)
    {
        return new UserCredentialEntity
        {
            UserId = cred.UserId,
            FirstName = cred.FirstName,
            LastName = cred.LastName,
            Username = cred.Username,
            PasswordHash = cred.PasswordHash,
            Email = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public UserCredential ToCredential()
    {
        return new UserCredential(
            UserId,
            FirstName,
            LastName,
            Username,
            PasswordHash
        );
    }
}

// ================= STUDY PLAN =================
public class StudyPlanEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public UserCredentialEntity? User { get; set; }
    public ICollection<LearningProgressEntity> LearningUnits { get; set; }
        = new List<LearningProgressEntity>();
}

// ================= CHAT SESSION =================
public class ChatSessionEntity
{
    public Guid SessionId { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }

    public string Title { get; set; } = "New Chat";
    public string Topic { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public UserCredentialEntity? User { get; set; }
    public ICollection<ChatMessageEntity> Messages { get; set; } = new List<ChatMessageEntity>();
}

// ================= CHAT MESSAGE =================
public class ChatMessageEntity
{
    public Guid MessageId { get; set; } = Guid.NewGuid();
    public Guid SessionId { get; set; }
    public Guid UserId { get; set; }

    public string Sender { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public UserCredentialEntity? User { get; set; }
    public ChatSessionEntity? Session { get; set; }

    public ChatMessage ToChatMessage()
        => new ChatMessage { Sender = Sender, Text = Text };
}

// ================= LEARNING PROGRESS =================
public class LearningProgressEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }
    public Guid StudyPlanId { get; set; }   // ⚠️ WAR DER FEHLENDE TEIL

    public string Subject { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public string Chapter { get; set; } = string.Empty;

    public DateTime PlannedStart { get; set; }
    public DateTime PlannedEnd { get; set; }

    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public UserCredentialEntity? User { get; set; }
    public StudyPlanEntity? StudyPlan { get; set; }
}
