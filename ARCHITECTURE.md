# LernApp - Architektur-Dokumentation

## 🏗️ Systemarchitektur

Das Projekt folgt einer 4-schichtigen Architektur:

```
┌─────────────────────────────────────────┐
│    Präsentationsschicht (UI)            │
│   MainWindow.xaml / Views (Avalonia)    │
└──────────────────┬──────────────────────┘
                   │
┌──────────────────▼──────────────────────┐
│   ViewModel-Schicht (ReactiveUI)        │
│  LernplanViewModel, etc.                │
└──────────────────┬──────────────────────┘
                   │
┌──────────────────▼──────────────────────┐
│   Service-Schicht (Business Logic)      │
│  - LernplanService                      │
│  - AIService                            │
│  - DateiAnalyseService                  │
│  - UserService                          │
│  - UserAppSettingsService               │
└──────────────────┬──────────────────────┘
                   │
┌──────────────────▼──────────────────────┐
│   Datenhaltungsschicht (Data Access)    │
│  - LernAppDbContext (Entity Framework)  │
│  - Repositories (Repository Pattern)    │
│  - SQLite Database                      │
└─────────────────────────────────────────┘
```

## 📦 Komponenten

### 1. **DbContext** (`Data/LernAppDbContext.cs`)
Entity Framework Core Context für Datenbankverwaltung. Definiert alle DbSets und Relationen.

### 2. **Entity Models** (`Models/`)
- **User** - Benutzer mit Authentifizierung
- **LernEinheit** - Lerneinheiten pro Benutzer
- **Prompt** - KI-Prompts mit Responses
- **GenerierteCSV** - Von KI generierte CSV-Dateien
- **DateiAnalyse** - Analysen hochgeladener Dateien
- **UserEinstellung** - Benutzerpräferenzen

### 3. **Repository Pattern** (`Data/Repositories/`)

#### Generic Repository
```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
}
```

#### Spezifische Repositories
- **IUserRepository** - Nutzer mit Email-Lookup, Einstellungen
- **ILernEinheitRepository** - Lerneinheiten mit Filterung
- **IPromptRepository** - Prompts mit Kategorisierung
- **IGenerierteCSVRepository** - CSV-Dateien verwalten

### 4. **Services** (`Services/`)

#### ILernplanService
```csharp
public interface ILernplanService
{
    Task<LernEinheit> ErstelleLernEinheitAsync(int userId, string fach, string thema, string? beschreibung);
    Task<IEnumerable<LernEinheit>> HoleLernEinheitenAsync(int userId);
    Task<LernEinheit?> HoleLernEinheitAsync(int id);
    Task<IEnumerable<LernEinheit>> HoleLernEinheitenNachFachAsync(string fach, int userId);
    Task<LernEinheit> AktualisiereLernEinheitAsync(LernEinheit lernEinheit);
    Task LöscheLernEinheitAsync(int id);
}
```

#### IAIService
```csharp
public interface IAIService
{
    Task<string> GeneriereLernplanAsync(string userInput, int userId);
    Task<Prompt> SpeicherePromptAsync(int userId, string text, string response, string? kategorie);
    Task<IEnumerable<Prompt>> HolePromptsAsync(int userId);
    Task<IEnumerable<Prompt>> HolePromptsNachKategorieAsync(string kategorie, int userId);
}
```

#### IDateiAnalyseService
```csharp
public interface IDateiAnalyseService
{
    Task<DateiAnalyse> AnalysiereDateiAsync(int lernEinheitId, string dateiname, string inhalt);
    Task<DateiAnalyse?> HoleDateiAnalyseAsync(int id);
    Task<IEnumerable<DateiAnalyse>> HoleDateiAnalysenAsync(int lernEinheitId);
}
```

#### IUserService
```csharp
public interface IUserService
{
    Task<User> RegisteriereBenutzerAsync(string name, string email, string passwordHash);
    Task<User?> AuthentifiziereBenutzerAsync(string email, string passwordHash);
    Task<User?> HoleBenutzerAsync(int userId);
    Task<User> AktualisiereBenutzerAsync(User user);
}
```

#### IUserAppSettingsService
```csharp
public interface IUserAppSettingsService
{
    Task<UserEinstellung> HoleEinstellungenAsync(int userId);
    Task<UserEinstellung> AktualisiereEinstellungenAsync(UserEinstellung einstellung);
}
```

## 🔧 Dependency Injection Setup

Das System ist in `Program.cs` konfiguriert:

```csharp
services.AddDbContext<LernAppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// Repositories
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<ILernEinheitRepository, LernEinheitRepository>();
services.AddScoped<IPromptRepository, PromptRepository>();
services.AddScoped<IGenerierteCSVRepository, GenerierteCSVRepository>();

// Services
services.AddScoped<ILernplanService, LernplanService>();
services.AddScoped<IAIService, AIService>();
services.AddScoped<IDateiAnalyseService, DateiAnalyseService>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<IUserAppSettingsService, UserAppSettingsService>();
```

Die Datenbank wird automatisch bei Startup erstellt: `dbContext.Database.EnsureCreated();`

## 📝 Beispiele der Verwendung

### Benutzer erstellen
```csharp
var userService = Program.Services?.GetRequiredService<IUserService>();
var user = await userService.RegisteriereBenutzerAsync(
    name: "Max Mustermann",
    email: "max@example.com",
    passwordHash: "hashed_password"
);
```

### Lerneinheit erstellen
```csharp
var lernplanService = Program.Services?.GetRequiredService<ILernplanService>();
var einheit = await lernplanService.ErstelleLernEinheitAsync(
    userId: user.Id,
    fach: "Mathematik",
    thema: "Integralrechnung",
    beschreibung: "Grundlagen der Integralrechnung"
);
```

### KI-Prompt speichern
```csharp
var aiService = Program.Services?.GetRequiredService<IAIService>();
var prompt = await aiService.SpeicherePromptAsync(
    userId: user.Id,
    text: "Erstelle einen Lernplan für Integralrechnung",
    response: "Hier ist ein KI-generierter Lernplan...",
    kategorie: "Lernplan"
);
```

### Dateianalyse durchführen
```csharp
var dateiService = Program.Services?.GetRequiredService<IDateiAnalyseService>();
var analyse = await dateiService.AnalysiereDateiAsync(
    lernEinheitId: einheit.Id,
    dateiname: "skript.pdf",
    inhalt: "Dateibuffer..."
);
```

### Benutzereinstellungen abrufen
```csharp
var settingsService = Program.Services?.GetRequiredService<IUserAppSettingsService>();
var einstellungen = await settingsService.HoleEinstellungenAsync(userId: user.Id);
einstellungen.Sprache = "en";
einstellungen.Thema = "dark";
await settingsService.AktualisiereEinstellungenAsync(einstellungen);
```

## 🗄️ Datenbank-Relationen

```
User (1) ──── (n) LernEinheit
User (1) ──── (n) Prompt
User (1) ──── (n) GenerierteCSV
User (1) ──── (1) UserEinstellung
LernEinheit (1) ──── (n) DateiAnalyse
Prompt (1) ──── (0..1) GenerierteCSV
```

## 🚀 Nächste Schritte

1. **Python-KI Integration**: Implementierung der Python-Scriptaufrufe in `AIService.RufeAIPythonScriptAsync()`
2. **ViewModels**: Verbindung der ViewModels mit den Services
3. **UI-Integration**: Binding der Views zu ViewModels
4. **Authentifizierung**: Implementierung von Passwort-Hashing (z.B. mit BCrypt)
5. **Validierung**: Input-Validierung in den Services
6. **Error Handling**: Erweiterte Fehlerbehandlung und benutzerfreundliche Fehlermeldungen
7. **Web-Unterstützung**: Adapter für Web-Version (ASP.NET Core)

## 📊 Logging

Das System verwendet eine einfache `ConsoleLogger` Implementierung. Diese kann erweitert werden zu:
- Serilog
- NLog
- Application Insights

