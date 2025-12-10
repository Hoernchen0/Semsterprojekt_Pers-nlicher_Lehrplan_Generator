using Avalonia.Controls;
using LehrplanGenerator.ViewModels;

namespace LehrplanGenerator.Views.Windows;

public partial class MenuView : UserControl
{
    private readonly MainWindow _mainWindow;

    public MenuView(MainWindow mainWindow)
    {
        InitializeComponent();
        _mainWindow = mainWindow;
        DataContext = new MenuViewModel();
    }
}
