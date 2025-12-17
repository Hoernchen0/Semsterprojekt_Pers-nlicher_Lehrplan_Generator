using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls;
using LehrplanGenerator.Models.Chat;
using LehrplanGenerator.Views.Windows;

namespace LehrplanGenerator.ViewModels.Chat;

public partial class ChatViewModel : ObservableObject
{
    private readonly MainWindow _mainWindow;

    public ObservableCollection<ChatMessage> Messages { get; } = new();

    [ObservableProperty]
    private string inputText = "";

    public ChatViewModel(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;

        Messages.Add(new ChatMessage
        {
            Sender = "KI",
            Text = "Hey! Wie kann ich dir helfen?"
        });
    }

    // Nachricht senden
    [RelayCommand]
    private async Task Send()
    {
        if (string.IsNullOrWhiteSpace(InputText))
            return;

        var userMsg = InputText;

        Messages.Add(new ChatMessage
        {
            Sender = "Du",
            Text = userMsg
        });

        InputText = "";

        await Task.Delay(250);

        Messages.Add(new ChatMessage
        {
            Sender = "KI",
            Text = $"Alles klar, du hast gesagt: {userMsg}"
        });
    }

    // Datei hochladen
    [RelayCommand]
    private async Task Upload()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Datei auswählen",
            AllowMultiple = false
        };

        var files = await dialog.ShowAsync(_mainWindow);

        if (files == null || files.Length == 0)
            return;

        string file = files[0];

        Messages.Add(new ChatMessage
        {
            Sender = "Du",
            Text = $"📎 Datei hochgeladen:\n{file}"
        });

        await Task.Delay(200);

        Messages.Add(new ChatMessage
        {
            Sender = "KI",
            Text = "Danke! Ich habe deine Datei verarbeitet."
        });
    }
}
