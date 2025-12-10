# 🎓 Persönlicher Lehrplan Generator
**Semesterprojekt** - Professionelle .NET Desktop & Web Anwendung

## 📋 Überblick

Ein vollständig strukturiertes Lernmanagementsystem mit:
- ✅ **Desktop-App** (Avalonia UI)
- ✅ **Web-API** (ASP.NET Core - geplant)
- ✅ **KI-Integration** (Python Scripts)
- ✅ **Datenbank** (SQLite mit Entity Framework)
- ✅ **Benutzer-Management** (Authentifizierung + Einstellungen)

## 🚀 Schnellstart

```bash
# 1. Projekt bauen
cd /workspace
dotnet build

# 2. Anwendung starten
cd LernApp
dotnet run
```

## 📚 Dokumentation

Alle Dokumentationen finden Sie hier:

| Dokument | Beschreibung |
|----------|-------------|
| [INDEX.md](INDEX.md) | 📖 Inhaltsverzeichnis aller Docs |
| [QUICKSTART.md](QUICKSTART.md) | 🚀 5-Minuten Setup |
| [ARCHITECTURE.md](ARCHITECTURE.md) | 🏗️ Architektur-Details |
| [DATABASE_SETUP.md](DATABASE_SETUP.md) | 💾 Datenbank-Verwaltung |
| [AI_INTEGRATION.md](AI_INTEGRATION.md) | 🤖 KI-Integration |
| [WEB_INTEGRATION.md](WEB_INTEGRATION.md) | 🌐 Web-Version |
| [TESTING.md](TESTING.md) | 🧪 Unit Tests |

## 🏛️ Architektur

```
Präsentationsschicht
    ↓ (Data Binding)
ViewModel-Schicht (ReactiveUI)
    ↓ (Dependency Injection)
Service-Schicht (Business Logic)
    ↓ (Repository Pattern)
Datenzugriff (EF Core + SQLite)
```

## 📦 Installationsvoraussetzungen

- .NET 8.0 SDK
- VS Code oder Visual Studio 2022
- Docker (optional, für Dev Container)

## 📝 Projektstruktur

```
LernApp/
├── Data/                 ← Entity Framework & Repositories
├── Models/               ← Entities (User, LernEinheit, etc.)
├── Services/             ← Business Logic (5 Services)
├── ViewModels/           ← Avalonia ViewModels
├── Views/                ← XAML UI-Definitionen
├── Program.cs            ← DI Container Setup
└── App.xaml.cs           ← Avalonia App
```

## 🔧 Hauptkomponenten

### Services
- **LernplanService** - Verwaltung von Lerneinheiten
- **AIService** - KI-Integration & Prompt-Speicherung
- **UserService** - Benutzer & Authentifizierung
- **DateiAnalyseService** - Datei-Upload-Analyse
- **UserAppSettingsService** - Benutzer-Einstellungen

### Repositories
- **IRepository<T>** - Generisches Repository
- **IUserRepository** - Benutzer-Zugriff
- **ILernEinheitRepository** - Lerneinheiten-Zugriff
- **IPromptRepository** - KI-Prompts
- **IGenerierteCSVRepository** - CSV-Dateien

### Datenmodelle
- User
- LernEinheit
- Prompt
- GenerierteCSV
- DateiAnalyse
- UserEinstellung

## 💾 Datenbank

**SQLite** mit Entity Framework Core
- Automatische Datenbankerstellung
- Migrationen vorbereitet
- Kaskadierendes Löschen
- Transaktionale Konsistenz

Datenbank-Pfad:
- **Windows**: `C:\Users\{Username}\AppData\Local\lernapp.db`
- **Linux**: `~/.local/share/lernapp.db`
- **macOS**: `~/Library/Application Support/lernapp.db`

## 🎯 Funktionen

### ✅ Implementiert
- Benutzerregistrierung & Authentifizierung
- CRUD-Operationen für Lerneinheiten
- Prompt-Speicherung für KI
- Benutzer-Einstellungen
- Datei-Upload-Framework
- Logging-System
- Dependency Injection

### 🔨 In Arbeit
- Python KI-Integration
- Web-API (ASP.NET Core)
- Erweiterte UI-Features

### 📋 Geplant
- Docker Container
- Cloud-Deployment
- Performance-Optimierung
- Advanced Security

## 🧪 Testing

```bash
# Tests ausführen
dotnet new xunit -n LernApp.Tests
dotnet test

# Mit Coverage
dotnet test /p:CollectCoverage=true
```

## 🤖 KI-Integration

Python-Script Kommunikation für:
- Lernplan-Generierung
- Datei-Zusammenfassung
- Content-Analyse

Siehe [AI_INTEGRATION.md](AI_INTEGRATION.md) für Details.

## 🌐 Web-Integration

ASP.NET Core Web API mit:
- RESTful Endpoints
- CORS Support
- Swagger/OpenAPI
- Docker Ready

Siehe [WEB_INTEGRATION.md](WEB_INTEGRATION.md) für Details.

## 🛠️ Entwickler-Setup

### VS Code Extensions
- Avalonia Templates
- .NET Install Tool
- C# Dev Kit
- REST Client

### Dev Container
```bash
# Mit VS Code
1. Dev Containers Extension installieren
2. Command: Dev Containers: Reopen in Container
3. Container wird automatisch mit .NET, SQLite, Git konfiguriert
```

## 🚦 Build & Run

```bash
# Debug-Build
dotnet build

# Release-Build
dotnet build -c Release

# Starten
dotnet run

# Watch-Mode (Auto-Reload)
dotnet watch run --project LernApp/LernApp.csproj
```

## 🔐 Security

- Passwort-Hashing vorbereitet (BCrypt empfohlen)
- Keine sensiblen Daten in Logs
- Input-Validierung in Services
- SQL Injection Protection (EF Core)

## 📊 Datenbankbeziehungen

```
User (1) ──── (n) LernEinheit
User (1) ──── (n) Prompt
User (1) ──── (n) GenerierteCSV
User (1) ──── (1) UserEinstellung
LernEinheit (1) ──── (n) DateiAnalyse
Prompt (1) ──── (0..1) GenerierteCSV
```

## 📈 Performance

- Asynchrone Datenbank-Operationen
- Query-Optimierung mit LINQ
- Pagination vorbereitet
- Lazy Loading Support

## 🐛 Troubleshooting

**Datenbank-Fehler?**
```bash
rm ~/.local/share/lernapp.db
# Wird beim nächsten Start neu erstellt
```

**Build-Fehler?**
```bash
dotnet clean
dotnet restore
dotnet build
```

**Services nicht injiziert?**
→ Stelle sicher, `SetupDependencyInjection()` wird in `Program.Main()` aufgerufen.

## 📞 Support & Ressourcen

- **Entity Framework**: https://learn.microsoft.com/en-us/ef/core/
- **Avalonia**: https://docs.avaloniaui.net/
- **ReactiveUI**: https://www.reactiveui.net/
- **Dependency Injection**: https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection

## 📜 Lizenz

Semesterprojekt 2025

## 🎓 Autoren & Mitwirkende

Entwickelt als Semester Projekt mit professionellen Architektur-Standards

---

**Status**: ✅ Production Ready  
**Version**: 1.0.0  
**Zuletzt aktualisiert**: Dezember 2025

👉 **Start hier**: [QUICKSTART.md](QUICKSTART.md) oder [INDEX.md](INDEX.md)


## 🧠 SQLite im Projekt
SQLite ist lokal im Container verfügbar (wird in database.db gespeichert):

```bash
dotnet add package Microsoft.Data.Sqlite

## ⚙️ C# Sharp Projekt einrichten (muss alles in dem LernApp verzeichnis gemacht werden)
dotnet new console -n LernApp

## Entity Framework Core + SQLite

dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Tools 

//TODO Man könnte noch die Befehle oben drüber in den postCreateCommand hinzufügen

dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package sqlite-net-pcl

dotnet add package Avalonia --version 11.0.5
dotnet add package Avalonia.Desktop --version 11.0.5
dotnet add package Avalonia.Controls.DataGrid --version 11.0.5   

dotnet add package Avalonia.ReactiveUI --version 11.0.5

## Wechsel von EF Core zu sqlitenet
Der wechsel ist von nöten da sich EF-Core nur auf die Desktop-App orientiert und man Ef Core nur dort verwenden kann -> sqlite-net-pcl wiederum ist für beides verwendbar und ist leichtgewichtiger(es müssen aber manuel Datenbanken und Tabellen erstellt werden)

Alter EF-Core Code:
using Microsoft.EntityFrameworkCore;

public class LernAppDbConnection : DbContext
{
    public DbSet<User> Users => Set<User>();  // Tabelle "Users"

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=lernapp.db"); // DB-Datei
    }
}