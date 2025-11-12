using Avalonia.Controls;
using LernApp.ViewModels;

namespace LernApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Add_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is LernplanViewModel vm)
        {
            vm.NeueEinheit(FachBox.Text ?? "", ThemaBox.Text ?? "");
            //vm.NeueEinheit(FachBox.Text, ThemaBox.Text);
            FachBox.Text = "";
            ThemaBox.Text = "";
        }
    }
}
