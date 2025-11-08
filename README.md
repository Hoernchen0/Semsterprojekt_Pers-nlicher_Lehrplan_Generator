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


## 🧠 SQLite in deinem Projekt
SQLite ist lokal im Container verfügbar (wird in database.db gespeichert):

```bash
dotnet add package Microsoft.Data.Sqlite

## ⚙️ C# Sharp Projekt einrichten
dotnet new console -n LernApp

## Entity Framework Core + SQLite

dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Tools 

//TODO Man könnte noch die Befehle oben drüber in den postCreateCommand hinzufügen