using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.ViewModels.Windows;
using LehrplanGenerator.ViewModels.Main;
using LehrplanGenerator.ViewModels.Auth;
using LehrplanGenerator.Views.Windows;
using LehrplanGenerator.ViewModels.Shell;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.ViewModels.Settings;
using LehrplanGenerator.ViewModels.Dashboard;
using LehrplanGenerator.ViewModels.Chat;
using LehrplanGenerator.ViewModels.StudyPlan;
using LehrplanGenerator.Logic;

namespace LehrplanGenerator;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();
        
        // Initialisiere die Datenbank (erstelle Tabellen, falls nicht vorhanden)
        ServiceExtensions.InitializeDatabase(Services);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Registriere SQLite-basierte Datenbank Services (alle Nutzerdaten, Chats, Tabellen)
        services.AddLehrplanServices();

        // Services
        services.AddSingleton<AppState>();
        services.AddSingleton<ViewLocator>();
        services.AddSingleton<INavigationService, NavigationService>();
        
        // UserCredentialStore wird jetzt über IUserRepository mit SQLite befüllt
        services.AddSingleton<UserCredentialStore>();

        // ViewModels
        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<RegisterViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<ShellViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<ChatViewModel>();
        services.AddTransient<StudyPlanViewModel>();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        DisableAvaloniaDataAnnotationValidation();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainWindowViewModel>()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new Views.Main.MainView
            {
                DataContext = Services.GetRequiredService<MainWindowViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}