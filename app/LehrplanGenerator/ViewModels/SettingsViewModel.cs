using CommunityToolkit.Mvvm.ComponentModel;
using LehrplanGenerator.Views.Windows;

namespace LehrplanGenerator.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly MainWindow _mainWindow;

    public SettingsViewModel(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
    }
}
