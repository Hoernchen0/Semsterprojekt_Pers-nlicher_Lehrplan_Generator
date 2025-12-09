using Avalonia.Controls;

namespace LehrplanGenerator.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        RegisterBtn.Click += (_, _) => SetView(new RegisterView());
    }

    private void SetView(Control view)
    {
        MainContent.Content = view;
    }
}
