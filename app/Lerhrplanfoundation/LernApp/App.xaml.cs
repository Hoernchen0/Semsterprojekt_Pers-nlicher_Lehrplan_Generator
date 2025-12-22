using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Controls.ApplicationLifetimes;
using LernApp.Services;
using LernApp.ViewModels;
using LernApp.Views;
using Microsoft.Extensions.DependencyInjection;

namespace LernApp;

public partial class App : Application
{
    private IClassicDesktopStyleApplicationLifetime? _lifetime;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _lifetime = desktop;
            ShowLoginWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ShowLoginWindow()
    {
        Console.WriteLine("🔐 ShowLoginWindow wird aufgerufen");
        var userService = Program.Services?.GetRequiredService<IUserService>();
        var logger = Program.Services?.GetRequiredService<ILernAppLogger>();

        var loginViewModel = new LoginViewModel(userService!, logger!);
        Console.WriteLine("✅ LoginViewModel erstellt");
        
        loginViewModel.LoginSuccessful += (userId) =>
        {
            Console.WriteLine($"⚡ LoginSuccessful Event empfangen mit userId={userId}");
            OnLoginSuccessful(userId);
        };
        Console.WriteLine("✅ LoginSuccessful Event-Handler registriert");

        var loginWindow = new LoginWindow
        {
            DataContext = loginViewModel
        };

        _lifetime!.MainWindow = loginWindow;
        Console.WriteLine("✅ LoginWindow als MainWindow gesetzt");
    }

    private void ShowMainWindow(int userId)
    {
        Console.WriteLine($"📄 ShowMainWindow wird aufgerufen für userId={userId}");
        
        var lernplanService = Program.Services?.GetRequiredService<ILernplanService>();
        var userService = Program.Services?.GetRequiredService<IUserService>();
        var aiService = Program.Services?.GetRequiredService<IAIService>();
        var logger = Program.Services?.GetRequiredService<ILernAppLogger>();

        var mainViewModel = new LernplanViewModel(lernplanService!, userService!, aiService!, logger!);

        // Lade Daten für Benutzer
        _ = mainViewModel.InitialisiereDatenAsync(userId);

        var mainWindow = new MainWindow
        {
            DataContext = mainViewModel
        };

        _lifetime!.MainWindow = mainWindow;
        Console.WriteLine($"✅ MainWindow wurde gesetzt");
    }

    private void OnLoginSuccessful(int userId)
    {
        Console.WriteLine($"🔔 App.OnLoginSuccessful aufgerufen mit userId={userId}");
        ShowMainWindow(userId);
    }
}
