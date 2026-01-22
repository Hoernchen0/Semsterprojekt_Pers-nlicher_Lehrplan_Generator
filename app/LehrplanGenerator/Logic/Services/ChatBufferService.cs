using System;
using System.Threading.Tasks;
using LehrplanGenerator.Logic.State;

namespace LehrplanGenerator.Logic.Services;

/// <summary>
/// Service zum Löschen des Chat-Buffers im RAM
/// (Nicht die Datenbank, nur die aktuelle Session im Memory)
/// </summary>
public class ChatBufferService
{
    private readonly AppState _appState;

    public ChatBufferService(AppState appState)
    {
        _appState = appState;
    }

    /// <summary>
    /// Löscht alle Chat-Nachrichten im RAM-Buffer (aktuelle Session)
    /// Datenbank bleibt erhalten!
    /// </summary>
    public void ClearChatBuffer()
    {
        _appState.ChatMessages.Clear();
        Console.WriteLine($"✓ Chat-Buffer geleert (RAM)");
    }

    /// <summary>
    /// Löscht Chat-Buffer und setzt Session zurück
    /// </summary>
    public void ClearChatBufferAndSession()
    {
        _appState.ChatMessages.Clear();
        Console.WriteLine($"✓ Chat-Buffer und Session zurückgesetzt");
    }

    /// <summary>
    /// Gibt Anzahl der Nachrichten im Buffer zurück
    /// </summary>
    public int GetBufferMessageCount()
    {
        return _appState.ChatMessages.Count;
    }
}
