using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using LernApp.Infrastructure;
using LernApp.Services;
using LernApp.Data;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
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
    }

    public override void OnFrameworkInitializationCompleted()
    {
        DisableAvaloniaDataAnnotationValidation();

        try
        {
            // Setup DI with LernApp core services + LehrplanGenerator-specific services
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

            // Determine DB path
            var dbPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                "lernapp.db");

            // Register LernApp services (DbContext, Repositories, Domain Services)
            services.AddApplicationServices(dbPath);

            // Register LehrplanGenerator-specific services and ViewModels
            services.AddSingleton<AppState>();
            services.AddSingleton<ViewLocator>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<AuthService>();
            
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

            Services = services.BuildServiceProvider();

            // Initialisiere die Datenbank
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<LernApp.Data.LernAppDbContext>();
                try
                {
                    // Erstelle die Datenbank falls sie nicht existiert
                    dbContext.Database.EnsureCreated();
                    System.Diagnostics.Debug.WriteLine("✅ Datenbank initialisiert erfolgreich");
                }
                catch (Exception dbEx)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Fehler bei Datenbankinitialisierung: {dbEx.Message}");
                    throw;
                }
            }

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Show LoginWindow first - user must login before accessing main app
                var loginVM = Services.GetRequiredService<LoginViewModel>();
                desktop.MainWindow = new Views.Windows.LoginWindow
                {
                    DataContext = loginVM
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                singleViewPlatform.MainView = new Views.Auth.LoginView
                {
                    DataContext = Services.GetRequiredService<LoginViewModel>()
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"FATAL ERROR in OnFrameworkInitializationCompleted: {ex}");
            throw;
        }
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