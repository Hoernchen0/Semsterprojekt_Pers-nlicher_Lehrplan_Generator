using Avalonia.Controls;
using LehrplanGenerator.ViewModels;
using LehrplanGenerator.Views.Windows;

namespace LehrplanGenerator.Views.Auth;

public partial class RegisterView : UserControl
{

    public RegisterView(MainWindow mainWindow)
    {
        InitializeComponent();
        DataContext = new RegisterViewModel(mainWindow);
    }
}