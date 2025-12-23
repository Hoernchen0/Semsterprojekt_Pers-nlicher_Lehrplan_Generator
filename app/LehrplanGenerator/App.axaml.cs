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
using LehrplanGenerator.Views.Windows;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.ViewModels.Auth;
using Avalonia.Controls;
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
        services.AddSingleton<AppState>();
        services.AddSingleton<ViewLocator>();
        services.AddSingleton<INavigationService, NavigationService>();

        // ViewModels
        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<LoginViewModel>();
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
            var mainWindowViewModel = Services.GetRequiredService<MainWindowViewModel>();

            var contentControl = new ContentControl
            {
                DataContext = mainWindowViewModel
            };

            contentControl.Bind(
                ContentControl.ContentProperty,
                new Avalonia.Data.Binding("CurrentViewModel")
            );

            singleViewPlatform.MainView = contentControl;
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