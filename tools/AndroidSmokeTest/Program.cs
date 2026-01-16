using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LernApp.Infrastructure;
using LernApp.Services;

namespace AndroidSmokeTest
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                Console.WriteLine("[Android SmokeTest] Teste LernApp Services für Android...");
                
                string dbPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "lernapp-android-smoketest.db");
                Console.WriteLine($"[Test] Verwende DB: {dbPath}");

                var services = new ServiceCollection();
                services.AddApplicationServices(dbPath);

                using var provider = services.BuildServiceProvider();
                using var scope = provider.CreateScope();

                // Test 1: DB erstellen
                var db = scope.ServiceProvider.GetRequiredService<LernApp.Data.LernAppDbContext>();
                await db.Database.EnsureCreatedAsync();
                Console.WriteLine("[✅] SQLite DB erstellt");

                // Test 2: User Service
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                var user = await userService.RegisteriereBenutzerAsync("Android Benutzer", "android@example.com", "android123");
                Console.WriteLine($"[✅] User registriert: {user.Email} (ID={user.Id})");

                // Test 3: Lernplan erstellen
                var lernplanService = scope.ServiceProvider.GetRequiredService<ILernplanService>();
                var lerneinheit = await lernplanService.ErstelleLernEinheitAsync(
                    user.Id, 
                    "Mathematik", 
                    "Trigonometrie", 
                    "Sinus und Kosinus"
                );
                Console.WriteLine($"[✅] Lerneinheit erstellt: {lerneinheit.Thema}");

                // Test 4: Prompt (KI-Anfrage) speichern
                var promptRepo = scope.ServiceProvider.GetRequiredService<LernApp.Data.Repositories.IPromptRepository>();
                var prompt = new LernApp.Models.Prompt
                {
                    UserId = user.Id,
                    Text = "Erkläre Sinus-Funktion",
                    Response = "Die Sinus-Funktion ist...",
                    Kategorie = "Mathematik",
                    ErstelltAm = DateTime.Now
                };
                await promptRepo.AddAsync(prompt);
                Console.WriteLine($"[✅] Prompt/KI-Anfrage gespeichert");

                // Test 5: Daten abrufen
                var gespeicherteLerneinheiten = await lernplanService.HoleLernEinheitenAsync(user.Id);
                Console.WriteLine($"[✅] {gespeicherteLerneinheiten.Count()} Lerneinheiten in DB gespeichert");

                Console.WriteLine("\n✅ ALLE ANDROID-TESTS BESTANDEN!");
                Console.WriteLine("   Die SQLite-Datenbank und Services funktionieren in Android!");
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
