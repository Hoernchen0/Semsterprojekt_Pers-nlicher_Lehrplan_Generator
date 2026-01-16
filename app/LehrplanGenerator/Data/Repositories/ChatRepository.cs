using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LehrplanGenerator.Logic.Models;

namespace LehrplanGenerator.Data.Repositories;

public interface IChatRepository
{
    Task<ChatSessionEntity> CreateSessionAsync(Guid userId, string title, string topic = "");
    Task<ChatSessionEntity?> GetSessionAsync(Guid sessionId);
    Task<IEnumerable<ChatSessionEntity>> GetUserSessionsAsync(Guid userId);
    Task<ChatMessageEntity> SaveMessageAsync(Guid sessionId, Guid userId, string sender, string text);
    Task<IEnumerable<ChatMessageEntity>> GetSessionMessagesAsync(Guid sessionId);
    Task<ChatSessionEntity> UpdateSessionAsync(ChatSessionEntity session);
}

public class ChatRepository : IChatRepository
{
    private readonly LehrplanDbContext _context;

    public ChatRepository(LehrplanDbContext context)
    {
        _context = context;
    }

    public async Task<ChatSessionEntity> CreateSessionAsync(Guid userId, string title, string topic = "")
    {
        var session = new ChatSessionEntity
        {
            SessionId = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Topic = topic,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ChatSessions.Add(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<ChatSessionEntity?> GetSessionAsync(Guid sessionId)
    {
        return await _context.ChatSessions
            .Include(s => s.Messages)
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);
    }

    public async Task<IEnumerable<ChatSessionEntity>> GetUserSessionsAsync(Guid userId)
    {
        return await _context.ChatSessions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.UpdatedAt)
            .ToListAsync();
    }

    public async Task<ChatMessageEntity> SaveMessageAsync(Guid sessionId, Guid userId, string sender, string text)
    {
        var message = new ChatMessageEntity
        {
            MessageId = Guid.NewGuid(),
            SessionId = sessionId,
            UserId = userId,
            Sender = sender,
            Text = text,
            CreatedAt = DateTime.UtcNow
        };

        _context.ChatMessages.Add(message);

        // Update session UpdatedAt
        var session = await _context.ChatSessions.FindAsync(sessionId);
        if (session != null)
        {
            session.UpdatedAt = DateTime.UtcNow;
            _context.ChatSessions.Update(session);
        }

        await _context.SaveChangesAsync();
        return message;
    }

    public async Task<IEnumerable<ChatMessageEntity>> GetSessionMessagesAsync(Guid sessionId)
    {
        return await _context.ChatMessages
            .Where(m => m.SessionId == sessionId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<ChatSessionEntity> UpdateSessionAsync(ChatSessionEntity session)
    {
        session.UpdatedAt = DateTime.UtcNow;
        _context.ChatSessions.Update(session);
        await _context.SaveChangesAsync();
        return session;
    }
}
