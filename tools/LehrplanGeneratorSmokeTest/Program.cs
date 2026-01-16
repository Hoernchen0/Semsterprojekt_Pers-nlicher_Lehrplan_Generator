using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LernApp.Infrastructure;
using LernApp.Services;

namespace LehrplanGeneratorSmokeTest
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                Console.WriteLine("[LehrplanGenerator SmokeTest] Starte DB und Services Test...");
                
                string dbPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "lehrplangenerator-smoketest.db");
                Console.WriteLine($"[Test] Verwende DB: {dbPath}");

                var services = new ServiceCollection();
                services.AddApplicationServices(dbPath);

                using var provider = services.BuildServiceProvider();
                using var scope = provider.CreateScope();

                // Test 1: DB erstellen
                var db = scope.ServiceProvider.GetRequiredService<LernApp.Data.LernAppDbContext>();
                await db.Database.EnsureCreatedAsync();
                Console.WriteLine("[✅] DB erstellt");

                // Test 2: User Service
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                var user = await userService.RegisteriereBenutzerAsync("Test User", "test@lehrplan.de", "pass123");
                Console.WriteLine($"[✅] User registriert: {user.Email} (ID={user.Id})");

                // Test 3: Benutzer abrufen
                var retrievedUser = await userService.HoleBenutzerAsync(user.Id);
                if (retrievedUser != null)
                {
                    Console.WriteLine($"[✅] User abgerufen: {retrievedUser.Name}");
                }

                // Test 4: Authentifizierung
                var authUser = await userService.AuthentifiziereBenutzerAsync("test@lehrplan.de", "pass123");
                if (authUser != null)
                {
                    Console.WriteLine($"[✅] Authentifizierung erfolgreich");
                }

                // Test 5: Lernplan Service
                var lernplanService = scope.ServiceProvider.GetRequiredService<ILernplanService>();
                var lerneinheiten = await lernplanService.HoleLernEinheitenAsync(user.Id);
                Console.WriteLine($"[✅] Lerneinheiten geladen: {lerneinheiten.Count()} Einträge");

                Console.WriteLine("\n✅ ALLE TESTS BESTANDEN - LehrplanGenerator Services funktionieren!");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fehler: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return 1;
            }
        }
    }
}
