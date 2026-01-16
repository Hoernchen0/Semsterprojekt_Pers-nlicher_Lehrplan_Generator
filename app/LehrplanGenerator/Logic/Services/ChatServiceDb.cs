using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Data.Repositories;

namespace LehrplanGenerator.Logic.Services;

/// <summary>
/// Service für die Verwaltung von AI-Chat-Sessions und Nachrichten
/// Speichert Chats in der SQLite-Datenbank
/// </summary>
public class ChatServiceDb
{
    private readonly IChatRepository _chatRepository;
    private Guid _currentSessionId = Guid.Empty;

    public ChatServiceDb(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    /// <summary>
    /// Erstellt eine neue Chat-Session für einen Benutzer
    /// </summary>
    public async Task<ChatSessionEntity> CreateSessionAsync(Guid userId, string title, string topic = "")
    {
        var session = await _chatRepository.CreateSessionAsync(userId, title, topic);
        _currentSessionId = session.SessionId;
        return session;
    }

    /// <summary>
    /// Lädt eine existierende Chat-Session
    /// </summary>
    public async Task<ChatSessionEntity?> GetSessionAsync(Guid sessionId)
    {
        return await _chatRepository.GetSessionAsync(sessionId);
    }

    /// <summary>
    /// Holt alle Chat-Sessions eines Benutzers
    /// </summary>
    public async Task<IEnumerable<ChatSessionEntity>> GetUserSessionsAsync(Guid userId)
    {
        return await _chatRepository.GetUserSessionsAsync(userId);
    }

    /// <summary>
    /// Speichert eine neue Nachricht in einem Chat
    /// </summary>
    public async Task<ChatMessageEntity> SaveMessageAsync(
        Guid sessionId, 
        Guid userId, 
        string sender, 
        string text)
    {
        _currentSessionId = sessionId;
        return await _chatRepository.SaveMessageAsync(sessionId, userId, sender, text);
    }

    /// <summary>
    /// Holt alle Nachrichten einer Session
    /// </summary>
    public async Task<IEnumerable<ChatMessageEntity>> GetSessionMessagesAsync(Guid sessionId)
    {
        return await _chatRepository.GetSessionMessagesAsync(sessionId);
    }

    /// <summary>
    /// Speichert eine Nachricht in der aktuellen Session
    /// </summary>
    public async Task<ChatMessageEntity> SaveMessageToCurrentSessionAsync(
        Guid userId, 
        string sender, 
        string text)
    {
        if (_currentSessionId == Guid.Empty)
            throw new InvalidOperationException("Keine aktive Chat-Session. Bitte zuerst CreateSessionAsync aufrufen.");

        return await _chatRepository.SaveMessageAsync(_currentSessionId, userId, sender, text);
    }

    /// <summary>
    /// Aktualisiert den Titel oder Thema einer Session
    /// </summary>
    public async Task<ChatSessionEntity> UpdateSessionAsync(
        Guid sessionId, 
        string? newTitle = null, 
        string? newTopic = null)
    {
        var session = await _chatRepository.GetSessionAsync(sessionId);
        if (session == null)
            throw new InvalidOperationException("Session nicht gefunden");

        if (!string.IsNullOrWhiteSpace(newTitle))
            session.Title = newTitle;

        if (!string.IsNullOrWhiteSpace(newTopic))
            session.Topic = newTopic;

        return await _chatRepository.UpdateSessionAsync(session);
    }

    /// <summary>
    /// Gibt die aktuelle Session-ID zurück
    /// </summary>
    public Guid GetCurrentSessionId() => _currentSessionId;

    /// <summary>
    /// Setzt eine neue aktuelle Session
    /// </summary>
    public void SetCurrentSession(Guid sessionId) => _currentSessionId = sessionId;
}
