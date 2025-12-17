using Avalonia.Controls;
namespace LehrplanGenerator.Views.Settings;

using LehrplanGenerator.ViewModels;

using LehrplanGenerator.Views.Windows;


public partial class SettingsView : UserControl
{
    private readonly MainWindow _main;


    public SettingsView(MainWindow mainWindow)
    {
        InitializeComponent();
        _main = mainWindow;
        DataContext = new SettingsViewModel(mainWindow);
    }
}