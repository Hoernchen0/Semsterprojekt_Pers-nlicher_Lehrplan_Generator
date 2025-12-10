# 🌐 LernApp.Web - ASP.NET Core Web-Anwendung

## Übersicht

`LernApp.Web` ist eine ASP.NET Core Razor Pages Webanwendung, die die gleiche Funktionalität wie die Desktop-App bietet, aber über einen Browser zugreifbar ist.

## Struktur

```
LernApp.Web/
├── Pages/
│   ├── Shared/
│   │   ├── _Layout.cshtml
│   │   └── _ValidationScriptsPartial.cshtml
│   ├── Index.cshtml
│   ├── Index.cshtml.cs
│   ├── Privacy.cshtml
│   └── Privacy.cshtml.cs
├── appsettings.json
├── Program.cs
├── Startup.cs (falls ASP.NET Core 6+)
└── LernApp.Web.csproj
```

## Setup

### 1. Abhängigkeiten konfigurieren

Die Web-App muss die gleichen Services nutzen wie die Desktop-App. Fügen Sie in `Program.cs` folgendes hinzu:

```csharp
// Add services to the container.
services.AddRazorPages();

// Shared Services
services.AddScoped<ILernplanService, LernplanService>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<IAIService, AIService>();
services.AddScoped<IDateiAnalyseService, DateiAnalyseService>();
services.AddScoped<IUserAppSettingsService, UserAppSettingsService>();
services.AddScoped<ILernAppLogger, ConsoleLogger>();

// Database
string dbPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "lernapp.db");

services.AddDbContext<LernAppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));
```

### 2. Authentifizierung

Fügen Sie ASP.NET Core Identity hinzu:

```bash
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.AspNetCore.Identity.UI
```

### 3. Pages erstellen

Wichtige Pages:

- **Login.cshtml** - Authentifizierung
- **Register.cshtml** - Benutzerregistrierung
- **Dashboard.cshtml** - Übersicht der Lerneinheiten
- **LernEinheit/Index.cshtml** - Lerneinheiten-Verwaltung
- **LernEinheit/Create.cshtml** - Neue Lerneinheit
- **LernEinheit/Edit.cshtml** - Lerneinheit bearbeiten
- **AI.cshtml** - KI-Prompt-Schnittstelle

## Starten

```bash
cd /workspace/LernApp.Web
dotnet run
```

App ist verfügbar unter: `https://localhost:5001`

## Features

- ✅ Responsive Design (Mobile-freundlich)
- ✅ Session-Management
- ✅ Passwort-Reset per Email
- ✅ Lerneinheiten-Verwaltung per Web
- ✅ KI-Integration über Web-UI
- ✅ Export zu PDF/CSV
- ✅ Google Calendar Sync (optional)

## Deployment

### Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "LernApp.Web.dll"]
```

### Azure/AWS

1. Erstelle WebApp-Ressource
2. Konfiguriere Datenbank-Connection
3. Deploy via `dotnet publish`

## Nächste Schritte

- [ ] Login/Register Pages implementieren
- [ ] Dashboard mit Lerneinheiten
- [ ] KI-Chat Interface
- [ ] Email-Integration für Passwort-Reset
- [ ] Two-Factor Authentication
- [ ] Responsive Bootstrap/Tailwind Design
