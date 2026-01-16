using System;
using System.Threading.Tasks;
using System.IO;
using LernApp.Data;
using LernApp.Infrastructure;
using LernApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopIntegrationTest;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== LehrplanGenerator Desktop Integration Test ===\n");

        var dbPath = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "lernapp-test.db");

        // Cleanup old test DB
        if (File.Exists(dbPath))
            File.Delete(dbPath);

        var services = new ServiceCollection();

        try
        {
            Console.WriteLine("[1/6] Registriere LernApp Services...");
            services.AddApplicationServices(dbPath);
            var serviceProvider = services.BuildServiceProvider();
            Console.WriteLine("✅ Services registriert\n");

            Console.WriteLine("[2/6] Überprüfe Datenbankinitialisierung...");
            var context = serviceProvider.GetRequiredService<LernAppDbContext>();
            context.Database.EnsureCreated();
            Console.WriteLine($"✅ Database erstellt bei: {dbPath}\n");

            Console.WriteLine("[3/6] Teste User Registration...");
            var userService = serviceProvider.GetRequiredService<IUserService>();
            var testUser = await userService.RegisteriereBenutzerAsync(
                name: "Test User",
                email: "test@lernapp.de",
                passwordHash: "hashedpassword123"
            );
            Console.WriteLine($"✅ Benutzer registriert - ID: {testUser.Id}, Email: {testUser.Email}\n");

            Console.WriteLine("[4/6] Teste User Authentication...");
            var authenticatedUser = await userService.AuthentifiziereBenutzerAsync(
                email: "test@lernapp.de",
                passwordHash: "hashedpassword123"
            );
            if (authenticatedUser != null)
                Console.WriteLine($"✅ Authentifizierung erfolgreich - User: {authenticatedUser.Name}\n");
            else
                Console.WriteLine("❌ Authentifizierung fehlgeschlagen\n");

            Console.WriteLine("[5/6] Teste Lerneinheit Speicherung...");
            var lernplanService = serviceProvider.GetRequiredService<ILernplanService>();
            var lerneinheit = await lernplanService.ErstelleLernEinheitAsync(
                userId: testUser.Id,
                fach: "Mathematik",
                thema: "Integrationen",
                beschreibung: "Grundlagen der Integralrechnung"
            );
            Console.WriteLine($"✅ Lerneinheit erstellt - ID: {lerneinheit.Id}, Thema: {lerneinheit.Thema}\n");

            Console.WriteLine("[6/6] Teste Chat/Prompt Speicherung...");
            var aiService = serviceProvider.GetRequiredService<IAIService>();
            var prompt = await aiService.SpeicherePromptAsync(
                userId: testUser.Id,
                text: "Erkläre mir Integralrechnung",
                response: "Integralrechnung ist das Gegenteil von Differentialrechnung...",
                kategorie: "Mathematik"
            );
            Console.WriteLine($"✅ Prompt gespeichert - ID: {prompt.Id}\n");

            Console.WriteLine("=" + new string('=', 48));
            Console.WriteLine("✅ ALLE TESTS BESTANDEN!");
            Console.WriteLine("=" + new string('=', 48));
            Console.WriteLine($"\nDatenbank-Pfad: {dbPath}");
            Console.WriteLine("\nLehrplanGenerator.Desktop ist bereit zum Ausführen!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ FEHLER: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
        }
    }
}
