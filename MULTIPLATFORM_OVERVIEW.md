# 🚀 Multi-Platform LernApp - Vollständiger Überblick

## 📋 Architektur-Übersicht

Das LernApp-Projekt unterstützt jetzt **3 Plattformen**:

```
                    ┌─────────────────────────────┐
                    │   Shared Data Layer         │
                    │  (LernApp.Data + Models)    │
                    │   SQLite Database           │
                    └──────────┬──────────────────┘
                               │
            ┌──────────────────┼──────────────────┐
            │                  │                  │
         Desktop           Web                Mobile
      (Avalonia)      (ASP.NET Core)          (MAUI)
         Desktop        Blazor/Razor        Android/iOS
         Windows        Pages with C#         Web API
         Linux          Bootstrap             Calls
         macOS          REST API
            │                  │                  │
    LernApp.csproj   LernApp.Web.csproj   LernApp.Mobile.csproj
```

## 📱 Plattform-Vergleich

| Aspekt | Desktop (Avalonia) | Web (ASP.NET Core) | Mobile (MAUI) |
|--------|-------|----|----|
| **OS Support** | Windows, Linux, macOS | Alle (Browser) | Android, iOS |
| **Offline-Modus** | ✅ Vollständig | ❌ Nur mit PWA | ✅ SQLite lokal |
| **Performance** | ⚡ Native | ⚡ Server-Abhängig | ⚡ Native |
| **Deployment** | 📦 Installer | ☁️ Cloud/Docker | 📲 App Store |
| **Zielgruppe** | Desktop-User | Web-Browser | Mobile-User |
| **Database** | SQLite lokal | SQL Server/PostgreSQL | REST API → Server |
| **Authentifizierung** | Forms-basiert | ASP.NET Identity | JWT Token |

## 🏗️ Projektstruktur

```
/workspace/
├── LernApp/                      # ✅ Desktop (Avalonia)
│   ├── Data/
│   ├── Models/
│   ├── Services/
│   ├── ViewModels/
│   ├── Views/
│   └── Program.cs
│
├── LernApp.Web/                  # 🆕 Web (ASP.NET Core)
│   ├── Pages/
│   ├── Services/
│   ├── Models/
│   └── Program.cs
│
├── LernApp.Mobile/               # 🆕 Mobile (MAUI)
│   ├── Views/
│   ├── ViewModels/
│   ├── Services/
│   ├── Platforms/
│   └── MauiProgram.cs
│
└── LernApp.Shared/               # 🆕 Shared-Library
    ├── Models/
    ├── Interfaces/
    └── Utilities/
```

## 🔄 Datenaustausch

### Desktop-Version
```
LoginWindow → LoginViewModel → UserService → Repository → SQLite Database
```

### Web-Version
```
Login.cshtml → LoginPageModel → UserService → DbContext → SQL Server
```

### Mobile-Version
```
LoginPage → LoginViewModel → AuthService → REST API → LernApp.Web → Database
```

## 🛠️ Entwicklung nach Plattform

### Desktop (Avalonia)
**Status:** ✅ Funktionsfähig mit Login-System

```bash
cd /workspace
dotnet build
dotnet run --project LernApp/LernApp.csproj
```

**Features:**
- ✅ Login/Register
- ✅ Lerneinheiten-Verwaltung
- ✅ Lokale Datenbank
- ⚠️ KI-Integration (in Entwicklung)

### Web (ASP.NET Core Razor Pages)
**Status:** 🆕 Neu erstellt, nicht konfiguriert

```bash
cd /workspace/LernApp.Web
dotnet run
# http://localhost:5001
```

**Zu Implementieren:**
- [ ] Razor Pages für Dashboard
- [ ] Identity/Authentication
- [ ] REST API Endpoints
- [ ] Bootstrap-Layout
- [ ] Session-Management

### Mobile (MAUI)
**Status:** 🆕 Neu erstellt, nicht konfiguriert

```bash
cd /workspace/LernApp.Mobile
# Android
dotnet build -f net8.0-android
# iOS
dotnet build -f net8.0-ios
```

**Zu Implementieren:**
- [ ] MAUI Pages
- [ ] API-Services
- [ ] JWT Authentication
- [ ] Offline-Sync
- [ ] Push-Notifications

## 📡 API-Spezifikation (für Mobile)

Die Web-Version stellt REST API Endpoints bereit:

```
POST   /api/auth/login              → Authentifizierung
POST   /api/auth/register           → Registrierung
GET    /api/lerneinheiten           → Alle Lerneinheiten laden
GET    /api/lerneinheiten/{id}      → Einzelne Lerneinheit
POST   /api/lerneinheiten           → Neue Lerneinheit erstellen
PUT    /api/lerneinheiten/{id}      → Lerneinheit aktualisieren
DELETE /api/lerneinheiten/{id}      → Lerneinheit löschen
POST   /api/ai/chat                 → KI-Chat
GET    /api/user/settings           → Benutzereinstellungen
```

## 🔐 Authentifizierung nach Plattform

### Desktop: Forms-basiert
```csharp
// LoginViewModel.cs
var user = await _userService.AuthentifiziereBenutzerAsync(email, password);
if (user != null)
{
    LoginSuccessful?.Invoke(user.Id);  // → MainWindow wechsel
}
```

### Web: ASP.NET Core Identity
```csharp
// LoginPageModel.cs
var result = await _signInManager.PasswordSignInAsync(
    email, password, isPersistent: true, lockoutOnFailure: true);
if (result.Succeeded)
{
    return RedirectToPage("/Dashboard");
}
```

### Mobile: JWT Token
```csharp
// AuthService.cs
var token = await _httpClient.PostAsync("/api/auth/login", 
    new { email, password });
_secureStorage.SaveToken(token);
_httpClient.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", token);
```

## 📊 Deployment-Roadmap

### Phase 1: Desktop ✅
- ✅ Avalonia Desktop App
- ✅ Login-System
- ✅ SQLite Database

### Phase 2: Web (In Progress)
- [ ] ASP.NET Core Web API
- [ ] Razor Pages Frontend
- [ ] SQL Server/PostgreSQL
- [ ] Docker-Unterstützung

### Phase 3: Mobile (Planned)
- [ ] MAUI Android App
- [ ] MAUI iOS App
- [ ] Offline-Sync
- [ ] Push-Notifications

## 🚀 Schnellstart

### Alles starten

```bash
# Terminal 1: Desktop
cd /workspace
dotnet run --project LernApp/LernApp.csproj

# Terminal 2: Web
cd /workspace/LernApp.Web
dotnet run

# Terminal 3: Mobile (Emulator)
cd /workspace/LernApp.Mobile
dotnet build -f net8.0-android
```

### Datenbank-Management

Alle Plattformen nutzen die gleiche Datenbank:

```bash
# Datenbank zurücksetzen
rm ~/.local/share/lernapp.db

# Datenbank überprüfen
sqlite3 ~/.local/share/lernapp.db
.tables
.schema Users
```

## 🐛 Häufige Probleme

### Desktop-App zeigt Login-Fenster, aber Button funktioniert nicht
- ✅ Behoben: Click-Handler in LoginWindow.axaml.cs hinzugefügt
- Test: `dotnet run --project LernApp/LernApp.csproj`

### Web-App antwortet nicht
- [ ] Stelle sicher, dass Port 5001 frei ist
- [ ] Überprüfe appsettings.json
- [ ] Kontrolliere Datenbank-Connection

### Mobile-App kann nicht connecten
- [ ] Überprüfe API-URL in ApiService
- [ ] Stelle sicher dass Web-App läuft
- [ ] Überprüfe Firewall/Netzwerk

## 📚 Dokumentation

- [QUICKSTART.md](QUICKSTART.md) - Schnelleinstieg
- [ARCHITECTURE.md](ARCHITECTURE.md) - Architektur-Details
- [WEB_APP_SETUP.md](WEB_APP_SETUP.md) - Web-App Anleitung
- [MOBILE_APP_SETUP.md](MOBILE_APP_SETUP.md) - Mobile-App Anleitung
- [DATABASE_INTEGRATION_TEST.md](DATABASE_INTEGRATION_TEST.md) - DB-Tests

## ✨ Nächste Schritte

1. **Desktop (Priority: 🔴 Sofort)**
   - ✅ Fix Login-Button
   - [ ] Teste Registrierung
   - [ ] Implementiere Lerneinheiten-Verwaltung

2. **Web (Priority: 🟡 Diese Woche)**
   - [ ] REST API Endpoints
   - [ ] Razor Pages erstellen
   - [ ] Authentication implementieren

3. **Mobile (Priority: 🟢 Nächste Woche)**
   - [ ] API-Services
   - [ ] MAUI Pages
   - [ ] Offline-Sync

---

**Status:** 3 von 3 Plattformen existieren, Desktop ist funktionsfähig ✅
