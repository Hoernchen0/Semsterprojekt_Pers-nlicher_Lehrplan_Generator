using Avalonia.Controls;
using LehrplanGenerator.ViewModels;
using LehrplanGenerator.Views.Windows;

namespace LehrplanGenerator.Views.Auth;

public partial class LoginView : UserControl
{
    public LoginView(MainWindow mainWindow)
    {
        InitializeComponent();
        DataContext = new LoginViewModel(mainWindow);
    }
}