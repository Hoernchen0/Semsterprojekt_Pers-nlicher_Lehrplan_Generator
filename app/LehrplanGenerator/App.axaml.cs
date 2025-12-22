using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using LernApp.Services;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using LehrplanGenerator.ViewModels;
using LehrplanGenerator.Views.Main;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Views.Main;
using LehrplanGenerator.Views.Windows;

namespace LehrplanGenerator;

public partial class App : Application
{
    public static UserCredentialStore CredentialStore { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        CredentialStore = new UserCredentialStore();

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
            desktop.MainWindow = new LehrplanGenerator.Views.Windows.MainWindow()
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new LehrplanGenerator.Views.Main.MainView
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
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
