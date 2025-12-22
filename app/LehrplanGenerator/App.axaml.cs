using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using LernApp.Services;
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
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Services
        services.AddSingleton<UserCredentialStore>();
        services.AddSingleton<AppState>();
        services.AddSingleton<ViewLocator>();
        services.AddSingleton<INavigationService, NavigationService>();

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

        DisableAvaloniaDataAnnotationValidation();

        // Setup DI and reuse LernApp core services
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        // determine DB path for host app (uses LocalApplicationData by default)
        var dbPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "lernapp.db");

        // register LernApp services (repositories, DbContext, domain services)
        LernApp.Services.ServiceCollectionExtensions.AddLernAppServices(services, dbPath);

        // Register ViewModels from this app so they can get services via constructor
        services.AddTransient<MainViewModel>();

        var provider = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Use the Windows.MainWindow implementation for navigation compatibility
            //desktop.MainWindow = new LehrplanGenerator.Views.Windows.MainWindow()
            //{
            //    DataContext = provider.GetRequiredService<MainViewModel>()
                //TODO: switch to MainWindowViewModel?
                
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainWindowViewModel>()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            //singleViewPlatform.MainView = new LehrplanGenerator.Views.Main.MainView
            //{
            //    DataContext = provider.GetRequiredService<MainViewModel>()
            //TODO: switch to MainWindowViewModel?
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