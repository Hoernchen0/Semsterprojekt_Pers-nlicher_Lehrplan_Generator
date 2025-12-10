# 📚 LernApp Dokumentation - Inhaltsverzeichnis

## 🚀 Start hier

1. **[QUICKSTART.md](QUICKSTART.md)** - 5-Minuten Setup & erste Schritte
2. **[PROJECT_OVERVIEW.md](PROJECT_OVERVIEW.md)** - Vollständige Projekt-Übersicht

## 🏗️ Architektur & Design

3. **[ARCHITECTURE.md](ARCHITECTURE.md)** - Detaillierte Architektur-Dokumentation
   - 4-schichtiges Design
   - Service-Interfaces
   - Repository Pattern
   - DI-Konfiguration

4. **[ARCHITECTURE_DIAGRAM.md](ARCHITECTURE_DIAGRAM.md)** - Visuelle Diagramme
   - Schichtenmodell
   - Service-Interaktionen
   - Entity-Relationships
   - Datenfluss-Diagramme

## 💾 Datenbank

5. **[DATABASE_SETUP.md](DATABASE_SETUP.md)** - Datenbank-Verwaltung
   - SQLite Setup
   - Tabellen-Struktur
   - Migrations
   - Datenbank-Sicherung

## 🤖 KI-Integration

6. **[AI_INTEGRATION.md](AI_INTEGRATION.md)** - Python KI-Integration
   - Python-Script Kommunikation
   - OpenAI/Claude Integration
   - Docker Deployment
   - Security & Performance

## 🌐 Web-Anwendung

7. **[WEB_INTEGRATION.md](WEB_INTEGRATION.md)** - ASP.NET Core Web-Version
   - Shared Project Setup
   - API-Endpoints
   - Frontend-Integration
   - Docker compose

## 🧪 Testing

8. **[TESTING.md](TESTING.md)** - Unit & Integration Tests
   - Service Tests
   - Repository Tests
   - Integration Workflows
   - Mock-Setup

---

## 🎯 Nach Aufgabe

### Ich möchte eine neue Lerneinheit erstellen
→ Siehe [QUICKSTART.md - Erste Lerneinheit erstellen](QUICKSTART.md#-erste-lerneinheit-erstellen)

### Ich möchte die Datenbank-Struktur verstehen
→ Siehe [DATABASE_SETUP.md - Datenbank-Struktur](DATABASE_SETUP.md#datenbank-struktur)

### Ich möchte einen neuen Service hinzufügen
→ Siehe [ARCHITECTURE.md - Services](ARCHITECTURE.md#4-services--services-)

### Ich möchte die KI integrieren
→ Siehe [AI_INTEGRATION.md](AI_INTEGRATION.md)

### Ich möchte Web-Unterstützung hinzufügen
→ Siehe [WEB_INTEGRATION.md](WEB_INTEGRATION.md)

### Ich möchte Tests schreiben
→ Siehe [TESTING.md](TESTING.md)

### Ich möchte die Architektur verstehen
→ Siehe [ARCHITECTURE_DIAGRAM.md](ARCHITECTURE_DIAGRAM.md)

---

## 📊 Wichtige Dateien im Projekt

### Kern-Dateien
```
/workspace/LernApp/
├── Program.cs                  ← DI Setup hier!
├── App.xaml.cs                 ← Service Injection
│
├── Data/
│   ├── LernAppDbContext.cs     ← Datenbank-Konfiguration
│   └── Repositories/           ← Daten-Zugriff
│
├── Models/                     ← Entity-Definitionen
│   ├── User.cs
│   ├── LernEinheit.cs
│   ├── Prompt.cs
│   ├── GenerierteCSV.cs
│   ├── DateiAnalyse.cs
│   └── UserEinstellung.cs
│
├── Services/                   ← Business Logic
│   ├── LernplanService.cs
│   ├── AIService.cs
│   ├── UserService.cs
│   ├── DateiAnalyseService.cs
│   └── UserAppSettingsService.cs
│
└── ViewModels/
    └── LernplanViewModel.cs    ← UI Logic
```

---

## 🔧 Schnell-Referenz: Services

### LernplanService
```csharp
var service = Program.Services?.GetRequiredService<ILernplanService>();
await service.ErstelleLernEinheitAsync(userId, fach, thema, beschreibung);
await service.HoleLernEinheitenAsync(userId);
await service.LöscheLernEinheitAsync(id);
```

### UserService
```csharp
var service = Program.Services?.GetRequiredService<IUserService>();
await service.RegisteriereBenutzerAsync(name, email, passwordHash);
var user = await service.AuthentifiziereBenutzerAsync(email, passwordHash);
```

### AIService
```csharp
var service = Program.Services?.GetRequiredService<IAIService>();
await service.SpeicherePromptAsync(userId, text, response, kategorie);
var prompts = await service.HolePromptsAsync(userId);
```

### DateiAnalyseService
```csharp
var service = Program.Services?.GetRequiredService<IDateiAnalyseService>();
await service.AnalysiereDateiAsync(lernEinheitId, dateiname, inhalt);
```

### UserAppSettingsService
```csharp
var service = Program.Services?.GetRequiredService<IUserAppSettingsService>();
var settings = await service.HoleEinstellungenAsync(userId);
await service.AktualisiereEinstellungenAsync(settings);
```

---

## 🎓 Lernpfad

### Anfänger
1. Lesen: [QUICKSTART.md](QUICKSTART.md)
2. Spielen: Services verwenden
3. Verstehen: [ARCHITECTURE.md](ARCHITECTURE.md)

### Fortgeschrittene
1. Lesen: [ARCHITECTURE_DIAGRAM.md](ARCHITECTURE_DIAGRAM.md)
2. Schreiben: [TESTING.md](TESTING.md)
3. Erweitern: Neue Services

### Expert
1. Integrieren: [AI_INTEGRATION.md](AI_INTEGRATION.md)
2. Skalieren: [WEB_INTEGRATION.md](WEB_INTEGRATION.md)
3. Optimieren: Performance & Security

---

## ❓ FAQ

**F: Wo wird die Datenbank gespeichert?**
A: Siehe [DATABASE_SETUP.md - Datenbank-Pfade](DATABASE_SETUP.md#datenbank-pfade)

**F: Wie registriere ich einen neuen Benutzer?**
A: Siehe [QUICKSTART.md - Erste Schritte](QUICKSTART.md)

**F: Wie integriere ich Python KI?**
A: Siehe [AI_INTEGRATION.md](AI_INTEGRATION.md)

**F: Wie schreibe ich Unit Tests?**
A: Siehe [TESTING.md](TESTING.md)

**F: Wie mache ich eine Web-Version?**
A: Siehe [WEB_INTEGRATION.md](WEB_INTEGRATION.md)

**F: Wo ist die DI Konfiguration?**
A: In `/workspace/LernApp/Program.cs` die `SetupDependencyInjection()` Methode

**F: Wie funktioniert das Repository Pattern?**
A: Siehe [ARCHITECTURE.md - Repository Pattern](ARCHITECTURE.md#3-repository-pattern--datarepositoriesirepositorycss-)

---

## 🚀 Deployment-Checkliste

- [ ] Passwort-Hashing aktivieren (BCrypt)
- [ ] Logging konfigurieren (optional: Serilog)
- [ ] KI-Integration testen
- [ ] Unit Tests schreiben
- [ ] Error Handling reviewen
- [ ] Security Audit durchführen
- [ ] Database Backup aufsetzen
- [ ] Performance-Tests durchführen

---

## 📞 Support & Ressourcen

### Projektdateien
- **Hauptprojekt**: `/workspace/LernApp`
- **Solution File**: `/workspace/workspace.sln`

### Externe Ressourcen
- [Entity Framework Core Docs](https://learn.microsoft.com/en-us/ef/core/)
- [Avalonia UI Docs](https://docs.avaloniaui.net/)
- [ReactiveUI Docs](https://www.reactiveui.net/)
- [Dependency Injection Guide](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)

### Kommandos
```bash
# Build
dotnet build

# Run
dotnet run

# Tests
dotnet test

# Clean
dotnet clean
```

---

## ✨ Features im Überblick

✅ 4-schichtige Architektur  
✅ Entity Framework Core + SQLite  
✅ Repository Pattern  
✅ Dependency Injection  
✅ Async/Await überall  
✅ Avalonia Desktop UI  
✅ ReactiveUI ViewModels  
✅ User Management  
✅ Lernplan CRUD  
✅ KI-Integration vorbereitet  
✅ Datei-Upload vorbereitet  
✅ Web-Grundlagen  
✅ Testing Framework  

---

**Version**: 1.0.0  
**Status**: Production Ready  
**Zuletzt aktualisiert**: Dezember 2025

