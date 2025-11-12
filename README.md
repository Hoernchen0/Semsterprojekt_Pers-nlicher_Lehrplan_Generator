# Semsterprojekt_Pers-nlicher_Lehrplan_Generator
Semesterprojekt zu Erstellung eines Persönlichen Lehrplan Generators


Install der benötigten VScode extensions:
Avalonia Templates
.NET Install Tool


# Devcontainer: .NET + SQLite + GitHub SSH

Dieses Setup erlaubt dir, direkt aus dem Container mit GitHub zu arbeiten (Commit, Push, Pull), SQLite zu nutzen und .NET-Projekte zu bauen.


## 🚀 Nutzung
1. wsl2 installieren
2. Ubuntu aus dem Microsoft Store installieren und in der wsl verwenden
3. Docker oder Podman installieren
4. Stelle sicher, dass du **SSH-Keys** auf deinem Host eingerichtet hast (`~/.ssh/id_rsa` oder `id_ed25519`).
5. Öffne dein Projekt in **VS Code**.
6. Installiere die Erweiterung **Dev Containers**.
7. Führe in der Command Palette aus: **Dev Containers: Reopen in Container**.
8. Der Container bindet automatisch deinen `.ssh`-Ordner ein und erlaubt Git-Operationen mit SSH.

## Struktur im Projekt
Program.cs -> Main starten der App
Models -> Datenobjekte (nicht tatsächliche Gespeicherte Daten sondern mehr Methode die sie verwalten)
Services -> Die Logik hinter dem tatsächlichen Speichern der Daten
ViewModels-> Mittelschicht zwischen UI und den Daten(Services+Models)
View -> Das Frontend bzw. die UI


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