using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using LernApp;
using LernApp.Data;
using LernApp.Infrastructure;
using LernApp.Models;
using LernApp.Services;

class Program
{
    public static ServiceProvider? Services { get; private set; }

    static void Main(string[] args)
    {
        SetupDependencyInjection();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    static void SetupDependencyInjection()
    {
        var services = new ServiceCollection();

        // Konfiguriere alle Application-Services (DbContext, Repositories, Services)
        string dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "lernapp.db");

        services.AddApplicationServices(dbPath);

        Services = services.BuildServiceProvider();

        // Datenbank migrieren
        using (var scope = Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<LernAppDbContext>();
            
            try
            {
                dbContext.Database.EnsureCreated();
                Console.WriteLine("✅ Datenbank erstellt/überprüft");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fehler beim Erstellen der Datenbank: {ex.Message}");
                // Versuche aggressiv zu löschen
                try
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    Thread.Sleep(500);
                    
                    if (File.Exists(dbPath))
                    {
                        File.Delete(dbPath);
                        Console.WriteLine("🗑️  Datenbank gelöscht");
                    }
                    
                    // Versuche erneut
                    dbContext.Database.EnsureCreated();
                    Console.WriteLine("✅ Datenbank nach Neuversuch erstellt");
                }
                catch (Exception ex2)
                {
                    Console.WriteLine($"❌ Kritischer Fehler: {ex2.Message}");
                    throw;
                }
            }

            // Erstelle Test-Benutzer wenn noch keiner existiert
            try
            {
                if (!dbContext.Users.Any())
                {
                    var testUser = new User
                    {
                        Name = "Test Benutzer",
                        Email = "test@example.com",
                        PasswordHash = "password123",
                        ErstelltAm = DateTime.Now,
                        AktualisiertAm = DateTime.Now
                    };
                    dbContext.Users.Add(testUser);
                    dbContext.SaveChanges();
                    Console.WriteLine("✅ Test-Benutzer erstellt: test@example.com / password123");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️  Fehler beim Erstellen des Test-Benutzers: {ex.Message}");
            }
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
}
