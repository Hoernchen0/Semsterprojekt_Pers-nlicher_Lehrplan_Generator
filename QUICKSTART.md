# 🚀 Quick Start Guide - LernApp

## Installation & Erste Schritte

### Voraussetzungen
- .NET 8.0 SDK
- Visual Studio Code oder Visual Studio 2022
- Python 3.8+ (für KI-Features, optional)

### 1. Projekt bauen
```bash
cd /workspace
dotnet build
```

### 2. Anwendung starten
```bash
cd /workspace/LernApp
dotnet run
```

### 3. Erste Benutzer erstellen

```csharp
// In einem separaten Test-Script oder Hauptprogramm
var userService = Program.Services?.GetRequiredService<IUserService>();

var newUser = await userService.RegisteriereBenutzerAsync(
    name: "Test Benutzer",
    email: "test@example.com",
    passwordHash: "test123"  // In Real: BCrypt oder Argon2 verwenden!
);

Console.WriteLine($"Benutzer erstellt mit ID: {newUser.Id}");
```

## 📱 Erste Lerneinheit erstellen

```csharp
var lernplanService = Program.Services?.GetRequiredService<ILernplanService>();

var einheit = await lernplanService.ErstelleLernEinheitAsync(
    userId: newUser.Id,
    fach: "Mathematik",
    thema: "Integralrechnung",
    beschreibung: "Grundlagen der Integralrechnung"
);

Console.WriteLine($"Lerneinheit erstellt: {einheit.Thema}");
```

## 🤖 KI-Feature testen

```csharp
var aiService = Program.Services?.GetRequiredService<IAIService>();

// Prompt speichern
var prompt = await aiService.SpeicherePromptAsync(
    userId: newUser.Id,
    text: "Erstelle einen Lernplan für Integralrechnung",
    response: "Dies würde von der KI generiert",
    kategorie: "Lernplan"
);

// Alle Prompts eines Benutzers abrufen
var userPrompts = await aiService.HolePromptsAsync(newUser.Id);
Console.WriteLine($"Gespeicherte Prompts: {userPrompts.Count()}");
```

## 📊 Lerneinheiten anzeigen

```csharp
var lernplanService = Program.Services?.GetRequiredService<ILernplanService>();

var einheiten = await lernplanService.HoleLernEinheitenAsync(newUser.Id);

foreach (var einheit in einheiten)
{
    Console.WriteLine($"- {einheit.Fach}: {einheit.Thema}");
}
```

## 🔧 Wichtigste Services

### ILernplanService
```csharp
// Neue Lerneinheit
var einheit = await service.ErstelleLernEinheitAsync(userId, fach, thema, beschreibung);

// Alle Lerneinheiten eines Benutzers
var einheiten = await service.HoleLernEinheitenAsync(userId);

// Nach Fach filtern
var matheEinheiten = await service.HoleLernEinheitenNachFachAsync("Mathematik", userId);

// Aktualisieren
await service.AktualisiereLernEinheitAsync(einheit);

// Löschen
await service.LöscheLernEinheitAsync(einheitId);
```

### IUserService
```csharp
// Registrieren
var user = await service.RegisteriereBenutzerAsync(name, email, passwordHash);

// Authentifizieren
var authenticatedUser = await service.AuthentifiziereBenutzerAsync(email, passwordHash);

// Abrufen
var user = await service.HoleBenutzerAsync(userId);

// Aktualisieren
await service.AktualisiereBenutzerAsync(user);
```

### IAIService
```csharp
// Prompt speichern
var prompt = await service.SpeicherePromptAsync(userId, text, response, kategorie);

// Prompts abrufen
var prompts = await service.HolePromptsAsync(userId);
var kategoriePrompts = await service.HolePromptsNachKategorieAsync("Lernplan", userId);

// Lernplan generieren (mit KI)
var lernplan = await service.GeneriereLernplanAsync(userInput, userId);
```

### IDateiAnalyseService
```csharp
// Datei analysieren
var analyse = await service.AnalysiereDateiAsync(lernEinheitId, dateiname, inhalt);

// Analysen einer Lerneinheit abrufen
var analysen = await service.HoleDateiAnalysenAsync(lernEinheitId);
```

### IUserAppSettingsService
```csharp
// Einstellungen abrufen
var settings = await service.HoleEinstellungenAsync(userId);

// Ändern
settings.Sprache = "en";
settings.Thema = "dark";
await service.AktualisiereEinstellungenAsync(settings);
```

## 🗄️ Datenbank-Abfragen (SQLite CLI)

```bash
# Verbindung
sqlite3 ~/.local/share/lernapp.db

# Alle Tabellen
.tables

# Benutzer anzeigen
SELECT * FROM Users;

# Lerneinheiten eines Benutzers
SELECT * FROM LernEinheiten WHERE UserId = 1;

# Prompts mit Count
SELECT Kategorie, COUNT(*) as Anzahl FROM Prompts GROUP BY Kategorie;

# Beenden
.quit
```

## 🐛 Fehlersuche

### "Services not found" Fehler
→ Stelle sicher, dass `SetupDependencyInjection()` in `Program.Main()` aufgerufen wird

### Datenbank-Fehler
```bash
# Datenbank zurücksetzen
rm ~/.local/share/lernapp.db
# Die Datenbank wird beim nächsten Start neu erstellt
```

### Build fehlgeschlagen
```bash
# Clean rebuild
dotnet clean
dotnet restore
dotnet build
```

## 📝 Logging

Das System verwendet einen `ConsoleLogger`. Ausgaben werden auf der Console angezeigt:

```
[INFO] 2025-12-10 14:30:45 - Lerneinheit erstellt: 5 für Benutzer 1
[WARN] 2025-12-10 14:31:00 - Fach und Thema sind erforderlich
[ERROR] 2025-12-10 14:32:15 - Fehler bei der Authentifizierung: Email not found
```

## 🎯 Nächste Schritte

1. **Python KI-Integration**: Implementieren Sie `AIService.RufeAIPythonScriptAsync()`
2. **UI-Binding**: Verbinden Sie Views mit ViewModels
3. **Passwort-Hashing**: Nutzen Sie BCrypt statt Klartext
4. **Web-API**: Erstellen Sie ASP.NET Core API-Endpoints
5. **Validierung**: Fügen Sie Daten-Validierung hinzu
6. **Testing**: Schreiben Sie Unit Tests

## 📚 Weitere Ressourcen

- [ARCHITECTURE.md](ARCHITECTURE.md) - Architektur-Übersicht
- [DATABASE_SETUP.md](DATABASE_SETUP.md) - Datenbank-Details
- [AI_INTEGRATION.md](AI_INTEGRATION.md) - KI-Integration
- [WEB_INTEGRATION.md](WEB_INTEGRATION.md) - Web-Version

---

**Viel Erfolg beim Entwickeln!** 🎓

