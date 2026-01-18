using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Data.Repositories;

namespace LehrplanGenerator.Logic.Services;

/// <summary>
/// Service für die Verwaltung von AI-Chat-Sessions und Nachrichten
/// Erzwingt User-Isolation (kein User sieht fremde Chats)
/// </summary>
public class ChatServiceDb
{
    private readonly IChatRepository _chatRepository;
    private Guid _currentSessionId = Guid.Empty;
    private Guid _currentUserId = Guid.Empty;

    public ChatServiceDb(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    // =========================
    // SESSION
    // =========================
    public async Task<ChatSessionEntity> CreateSessionAsync(
        Guid userId,
        string title,
        string topic = "")
    {
        _currentUserId = userId;

        var session = await _chatRepository.CreateSessionAsync(userId, title, topic);
        _currentSessionId = session.SessionId;

        return session;
    }

    public async Task<ChatSessionEntity?> GetSessionAsync(
        Guid sessionId,
        Guid userId)
    {
        var session = await _chatRepository.GetSessionAsync(sessionId);

        if (session == null || session.UserId != userId)
            return null;

        _currentUserId = userId;
        _currentSessionId = sessionId;

        return session;
    }

    public async Task<IEnumerable<ChatSessionEntity>> GetUserSessionsAsync(Guid userId)
    {
        _currentUserId = userId;

        // 🔒 ABSOLUTE USER-FILTERUNG
        return (await _chatRepository.GetUserSessionsAsync(userId))
            .Where(s => s.UserId == userId);
    }

    // =========================
    // MESSAGES
    // =========================
    public async Task<ChatMessageEntity> SaveMessageAsync(
        Guid sessionId,
        Guid userId,
        string sender,
        string text)
    {
        var session = await _chatRepository.GetSessionAsync(sessionId);
        if (session == null || session.UserId != userId)
            throw new InvalidOperationException("Keine Berechtigung für diese Chat-Session");

        _currentUserId = userId;
        _currentSessionId = sessionId;

        return await _chatRepository.SaveMessageAsync(
            sessionId,
            userId,
            sender,
            text
        );
    }

    public async Task<IEnumerable<ChatMessageEntity>> GetSessionMessagesAsync(
        Guid sessionId,
        Guid userId)
    {
        var session = await _chatRepository.GetSessionAsync(sessionId);
        if (session == null || session.UserId != userId)
            return Enumerable.Empty<ChatMessageEntity>();

        _currentUserId = userId;
        _currentSessionId = sessionId;

        // 🔒 FILTER AUCH AUF MESSAGE-EBENE
        return (await _chatRepository.GetSessionMessagesAsync(sessionId))
            .Where(m => m.UserId == userId);
    }

    public async Task<ChatMessageEntity> SaveMessageToCurrentSessionAsync(
        Guid userId,
        string sender,
        string text)
    {
        if (_currentSessionId == Guid.Empty)
            throw new InvalidOperationException("Keine aktive Chat-Session");

        return await SaveMessageAsync(
            _currentSessionId,
            userId,
            sender,
            text
        );
    }

    // =========================
    // UPDATE SESSION
    // =========================
    public async Task<ChatSessionEntity> UpdateSessionAsync(
        Guid sessionId,
        Guid userId,
        string? newTitle = null,
        string? newTopic = null)
    {
        var session = await _chatRepository.GetSessionAsync(sessionId);
        if (session == null || session.UserId != userId)
            throw new InvalidOperationException("Keine Berechtigung für diese Session");

        if (!string.IsNullOrWhiteSpace(newTitle))
            session.Title = newTitle;

        if (!string.IsNullOrWhiteSpace(newTopic))
            session.Topic = newTopic;

        return await _chatRepository.UpdateSessionAsync(session);
    }

    // =========================
    // STATE
    // =========================
    public Guid GetCurrentSessionId() => _currentSessionId;

    public void SetCurrentSession(Guid sessionId, Guid userId)
    {
        _currentSessionId = sessionId;
        _currentUserId = userId;
    }
}
