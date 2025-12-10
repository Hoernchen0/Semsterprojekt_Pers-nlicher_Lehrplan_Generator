using Avalonia.Controls;
using LehrplanGenerator.Views.Windows;

using LehrplanGenerator.Views.Chat;
using LehrplanGenerator.Views.Main;
using LehrplanGenerator.Views.Settings;

namespace LehrplanGenerator.Views.Shell;



public partial class ShellView : UserControl
{
    private readonly MainWindow _mainWindow;

    public ShellView(MainWindow mainWindow)
    {
        InitializeComponent();

        _mainWindow = mainWindow;

        MenuBtn.Click += (_, _) => ShellContent.Content = new MenuView(mainWindow);
        ChatBtn.Click += (_, _) => ShellContent.Content = new ChatView(mainWindow);
        SettingsBtn.Click += (_, _) => ShellContent.Content = new SettingsView(mainWindow);

        ShellContent.Content = new MenuView(mainWindow);
    }
}
