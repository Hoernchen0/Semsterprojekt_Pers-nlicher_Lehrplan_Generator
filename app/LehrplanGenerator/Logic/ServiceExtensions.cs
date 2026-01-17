using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LehrplanGenerator.Data;
using LehrplanGenerator.Data.Repositories;
using LehrplanGenerator.Logic.Services;

namespace LehrplanGenerator.Logic;

/// <summary>
/// Dependency Injection Setup für LehrplanGenerator Library
/// Wird von Desktop-, Android-, iOS-, Browser- und Web-Projekten verwendet
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Registriert alle Datenbank-Services und Repositories
    /// </summary>
    public static IServiceCollection AddLehrplanServices(
        this IServiceCollection services,
        string? databasePath = null)
    {
        // Datenbank-Pfad bestimmen
        if (string.IsNullOrWhiteSpace(databasePath))
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            databasePath = Path.Combine(appDataPath, "LehrplanGenerator", "lehrplan.db");
        }

        // Ordner erstellen, falls nicht vorhanden
        var dbFolder = Path.GetDirectoryName(databasePath);
        if (!Directory.Exists(dbFolder))
            Directory.CreateDirectory(dbFolder!);

        // DbContext registrieren
        services.AddDbContext<LehrplanDbContext>(options =>
            options.UseSqlite($"Data Source={databasePath}"));

        // Repositories registrieren
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<ICalendarRepository, CalendarRepository>();

        // Services registrieren
        services.AddScoped<AuthServiceDb>();
        services.AddScoped<ChatServiceDb>();
        services.AddScoped<PersistenceService>();

        return services;
    }

    /// <summary>
    /// Initialisiert die Datenbank (erstellt Tabellen, falls nicht vorhanden)
    /// </summary>
    public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<LehrplanDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
        }
    }

    /// <summary>
    /// Initialisiert die Datenbank synchron
    /// </summary>
    public static void InitializeDatabase(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<LehrplanDbContext>();
            dbContext.Database.EnsureCreated();
        }
    }
}
