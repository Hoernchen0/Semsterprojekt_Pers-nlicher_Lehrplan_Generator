using Avalonia.Controls;
using LehrplanGenerator.Views.Auth;
using LehrplanGenerator.Views.Windows;

namespace LehrplanGenerator.Views.Main;

public partial class MainView : UserControl
{
    private readonly MainWindow? _mainWindow;

    // PARAMETERLOSER KONSTRUKTOR für App.axaml.cs
    public MainView()
    {
        InitializeComponent();
    }

    // KONSTRUKTOR mit MainWindow für Navigation
    public MainView(MainWindow mainWindow)
    {
        InitializeComponent();
        _mainWindow = mainWindow;

        LoginBtn.Click += (_, _) => _mainWindow.SetView(new LoginView(_mainWindow));
        RegisterBtn.Click += (_, _) => _mainWindow.SetView(new RegisterView(_mainWindow));
    }
}
