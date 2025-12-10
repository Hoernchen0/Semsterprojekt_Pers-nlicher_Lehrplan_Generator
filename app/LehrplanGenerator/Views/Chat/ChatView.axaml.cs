using Avalonia.Controls;
using LehrplanGenerator.ViewModels.Chat;
using LehrplanGenerator.Views.Windows;

namespace LehrplanGenerator.Views.Chat;

public partial class ChatView : UserControl
{
    private readonly MainWindow _main;

    public ChatView(MainWindow mainWindow)
    {
        InitializeComponent();
        _main = mainWindow;
        DataContext = new ChatViewModel(mainWindow);
    }
}
