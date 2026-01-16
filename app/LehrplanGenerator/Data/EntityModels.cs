using System;
using System.Collections.Generic;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Models.Chat;

namespace LehrplanGenerator.Logic.Models;

// Datenbank Entity für User (erweitert UserCredential)
public class UserCredentialEntity
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public ICollection<ChatSessionEntity> ChatSessions { get; set; } = new List<ChatSessionEntity>();
    public ICollection<ChatMessageEntity> ChatMessages { get; set; } = new List<ChatMessageEntity>();

    // Helper
    public string DisplayName => $"{FirstName} {LastName}";

    public static UserCredentialEntity FromCredential(UserCredential cred)
    {
        return new UserCredentialEntity
        {
            UserId = cred.UserId,
            FirstName = cred.FirstName,
            LastName = cred.LastName,
            Username = cred.Username,
            PasswordHash = cred.PasswordHash,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public UserCredential ToCredential()
    {
        return new UserCredential(UserId, FirstName, LastName, Username, PasswordHash);
    }
}

// Chat Session Entity
public class ChatSessionEntity
{
    public Guid SessionId { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Title { get; set; } = "New Chat";
    public string Topic { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public UserCredentialEntity? User { get; set; }
    public ICollection<ChatMessageEntity> Messages { get; set; } = new List<ChatMessageEntity>();
}

// Chat Message Entity (erweitert ChatMessage)
public class ChatMessageEntity
{
    public Guid MessageId { get; set; } = Guid.NewGuid();
    public Guid SessionId { get; set; }
    public Guid UserId { get; set; }
    public string Sender { get; set; } = string.Empty; // "User" oder "AI"
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public UserCredentialEntity? User { get; set; }
    public ChatSessionEntity? Session { get; set; }

    public ChatMessage ToChatMessage()
    {
        return new ChatMessage { Sender = Sender, Text = Text };
    }
}
