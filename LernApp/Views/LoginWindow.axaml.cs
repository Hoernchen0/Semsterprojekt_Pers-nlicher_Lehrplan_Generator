using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using LernApp.ViewModels;
using System;

namespace LernApp.Views;

public partial class LoginWindow : Window
{
    private Button? _loginButton;
    private Button? _registerButton;

    public LoginWindow()
    {
        InitializeComponent();
        
        // Nach InitializeComponent die Buttons suchen und anmelden
        Dispatcher.UIThread.Post(() =>
        {
            _loginButton = this.FindControl<Button>("LoginButton");
            _registerButton = this.FindControl<Button>("RegisterButton");
            
            if (_loginButton != null)
            {
                _loginButton.Click += LoginButton_Click;
                Console.WriteLine("✅ LoginButton Click-Handler angebunden");
            }
            else
            {
                Console.WriteLine("❌ LoginButton nicht gefunden!");
            }
            
            if (_registerButton != null)
            {
                _registerButton.Click += RegisterButton_Click;
                Console.WriteLine("✅ RegisterButton Click-Handler angebunden");
            }
            else
            {
                Console.WriteLine("❌ RegisterButton nicht gefunden!");
            }
        }, DispatcherPriority.Render);
    }

    private void LoginButton_Click(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("🔘 LoginButton clicked!");
        if (DataContext is LoginViewModel viewModel)
        {
            Console.WriteLine("🔐 Führe LoginCommand aus...");
            try
            {
                var result = viewModel.LoginCommand.Execute();
                Console.WriteLine($"📤 LoginCommand.Execute() returned: {result?.GetType().Name ?? "null"}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fehler beim Ausführen des LoginCommand: {ex.Message}");
                Console.WriteLine($"   {ex.StackTrace}");
            }
        }
        else
        {
            Console.WriteLine("❌ DataContext ist nicht LoginViewModel!");
        }
    }

    private void RegisterButton_Click(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("🔘 RegisterButton clicked!");
        if (DataContext is LoginViewModel viewModel)
        {
            Console.WriteLine("📝 Führe RegisterCommand aus...");
            try
            {
                var result = viewModel.RegisterCommand.Execute();
                Console.WriteLine($"📤 RegisterCommand.Execute() returned: {result?.GetType().Name ?? "null"}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fehler beim Ausführen des RegisterCommand: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("❌ DataContext ist nicht LoginViewModel!");
        }
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        Console.WriteLine($"📍 LoginWindow.OnDataContextChanged: DataContext={DataContext?.GetType().Name}");
    }
}
