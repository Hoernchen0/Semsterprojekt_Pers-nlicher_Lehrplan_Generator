using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LernApp.Infrastructure;
using LernApp.Services;

namespace LoginSmokeTest
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                string dbPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "lernapp-smoketest.db");
                Console.WriteLine($"[SmokeTest] 📁 Verwende DB: {dbPath}");

                var services = new ServiceCollection();
                services.AddApplicationServices(dbPath);

                using var provider = services.BuildServiceProvider();
                using var scope = provider.CreateScope();

                var db = scope.ServiceProvider.GetRequiredService<LernApp.Data.LernAppDbContext>();
                await db.Database.EnsureCreatedAsync();
                Console.WriteLine("[SmokeTest] ✅ DB erstellt/überprüft");

                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                var email = "smoketest@example.com";
                var name = "Smoke Tester";
                var password = "smokepass123";

                // Registrierung
                Console.WriteLine("[SmokeTest] Registrierung wird versucht...");
                try
                {
                    var registeredUser = await userService.RegisteriereBenutzerAsync(name, email, password);
                    Console.WriteLine($"[SmokeTest] ✅ Registrierung erfolgreich: UserId={registeredUser.Id}");
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("[SmokeTest] ⚠️  Benutzer existiert bereits");
                }

                // Anmeldung
                Console.WriteLine("[SmokeTest] Anmeldung wird versucht...");
                var user = await userService.AuthentifiziereBenutzerAsync(email, password);
                if (user != null)
                {
                    Console.WriteLine($"[SmokeTest] ✅ Anmeldung erfolgreich: UserId={user.Id}, Email={user.Email}");
                    Console.WriteLine("[SmokeTest] ✅ ALLE TESTS BESTANDEN");
                    return 0;
                }
                else
                {
                    Console.WriteLine("[SmokeTest] ❌ Anmeldung fehlgeschlagen");
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SmokeTest] ❌ Fehler: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return 2;
            }
        }
    }
}
