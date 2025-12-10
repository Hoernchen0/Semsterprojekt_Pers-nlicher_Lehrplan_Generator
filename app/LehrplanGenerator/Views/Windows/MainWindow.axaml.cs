using Avalonia.Controls;
using LehrplanGenerator.Views.Main;

namespace LehrplanGenerator.Views.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        RootContent.Content = new MainView(this);
    }

    public void SetView(UserControl view)
    {
        RootContent.Content = view;
    }
}
