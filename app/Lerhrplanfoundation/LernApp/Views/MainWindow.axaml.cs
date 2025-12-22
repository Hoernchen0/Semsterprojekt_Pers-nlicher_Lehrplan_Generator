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
            vm.SelectedFach = FachBox.Text ?? "";
            vm.NeuesThema = ThemaBox.Text ?? "";
            _ = vm.ErzeugeNeueLernEinheitCommand.Execute();
            FachBox.Text = "";
            ThemaBox.Text = "";
        }
    }
}
