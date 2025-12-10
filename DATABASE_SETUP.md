# Datenbanksetup und Migration Guide

## Entity Framework Core Migrations

Da wir `EnsureCreated()` verwenden, werden Migrations nicht benötigt, aber für Production empfohlen:

### Migrations erstellen (optional, für Production)

```bash
cd /workspace/LernApp
dotnet ef migrations add InitialCreate --output-dir Data/Migrations
```

### Migrations anwenden

```bash
dotnet ef database update
```

## SQLite Datenbank

### Datenbank-Pfad
```
Windows: C:\Users\{Username}\AppData\Local\lernapp.db
Linux:   ~/.local/share/lernapp.db
macOS:   ~/Library/Application Support/lernapp.db
```

### Datenbank-Dateigröße überprüfen

```bash
ls -lh ~/.local/share/lernapp.db
```

## Datenbank-Struktur

Die Datenbank wird automatisch mit folgenden Tabellen erstellt:

### Users
```sql
CREATE TABLE "Users" (
    "Id" INTEGER NOT NULL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "PasswordHash" TEXT NOT NULL,
    "LernzeitProTag" INTEGER NOT NULL,
    "ErstelltAm" TEXT NOT NULL,
    "AktualisiertAm" TEXT NOT NULL
);
```

### LernEinheiten
```sql
CREATE TABLE "LernEinheiten" (
    "Id" INTEGER NOT NULL PRIMARY KEY,
    "UserId" INTEGER NOT NULL,
    "Fach" TEXT NOT NULL,
    "Thema" TEXT NOT NULL,
    "Beschreibung" TEXT,
    "Datum" TEXT NOT NULL,
    "ErstelltAm" TEXT NOT NULL,
    "AktualisiertAm" TEXT NOT NULL,
    FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);
```

### Prompts
```sql
CREATE TABLE "Prompts" (
    "Id" INTEGER NOT NULL PRIMARY KEY,
    "UserId" INTEGER NOT NULL,
    "Text" TEXT NOT NULL,
    "Response" TEXT NOT NULL,
    "ErstelltAm" TEXT NOT NULL,
    "Kategorie" TEXT,
    FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);
```

### GenerierteCSVs
```sql
CREATE TABLE "GenerierteCSVs" (
    "Id" INTEGER NOT NULL PRIMARY KEY,
    "UserId" INTEGER NOT NULL,
    "PromptId" INTEGER,
    "Dateiname" TEXT NOT NULL,
    "Inhalt" TEXT NOT NULL,
    "ErstelltAm" TEXT NOT NULL,
    "Beschreibung" TEXT,
    FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE,
    FOREIGN KEY ("PromptId") REFERENCES "Prompts" ("Id") ON DELETE SET NULL
);
```

### DateiAnalysen
```sql
CREATE TABLE "DateiAnalysen" (
    "Id" INTEGER NOT NULL PRIMARY KEY,
    "LernEinheitId" INTEGER NOT NULL,
    "Dateiname" TEXT NOT NULL,
    "InhaltZusammenfassung" TEXT NOT NULL,
    "AnalysiertAm" TEXT NOT NULL,
    "DateityP" TEXT,
    FOREIGN KEY ("LernEinheitId") REFERENCES "LernEinheiten" ("Id") ON DELETE CASCADE
);
```

### UserEinstellungen
```sql
CREATE TABLE "UserEinstellungen" (
    "Id" INTEGER NOT NULL PRIMARY KEY,
    "UserId" INTEGER NOT NULL UNIQUE,
    "Sprache" TEXT NOT NULL,
    "Thema" TEXT NOT NULL,
    "BenachrichtigungenAktiv" INTEGER NOT NULL,
    "AIModell" TEXT,
    "AktualisiertAm" TEXT NOT NULL
);
```

## Datenbank-Management

### Datenbank über SQLite CLI verwalten

```bash
# Verbindung zur Datenbank herstellen
sqlite3 ~/.local/share/lernapp.db

# Tabellen anzeigen
.tables

# Schema einer Tabelle anzeigen
.schema Users

# Daten abfragen
SELECT * FROM Users;
SELECT COUNT(*) FROM LernEinheiten;

# Beenden
.quit
```

### Datenbank sichern

```bash
cp ~/.local/share/lernapp.db ~/.local/share/lernapp.db.backup
```

### Datenbank zurücksetzen (für Tests)

```bash
rm ~/.local/share/lernapp.db
# Nächster App-Start erstellt die Datenbank neu
```

## Performance Tipps

1. **Indizes erstellen** für häufig abgefragte Spalten
2. **Lazy Loading** vs **Eager Loading** richtig einsetzen
3. **Query Optimization**: Nur benötigte Spalten laden
4. **Pagination**: Bei vielen Datensätzen implementieren

Beispiel Pagination:

```csharp
public async Task<IEnumerable<LernEinheit>> GetPagedAsync(int userId, int pageNumber, int pageSize)
{
    return await _context.LernEinheiten
        .Where(l => l.UserId == userId)
        .OrderByDescending(l => l.Datum)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
}
```

## Datenbank-Backup-Strategie

### Automatisiertes Backup (Cron Job unter Linux)

```bash
0 2 * * * cp ~/.local/share/lernapp.db ~/.local/share/backups/lernapp.db.$(date +\%Y\%m\%d)
```

### Cloud-Backup (z.B. mit Azure Blob Storage)

```csharp
// Speichern Sie die Datenbank-Datei periodisch in der Cloud
var backupService = new BackupService();
await backupService.BackupToAzureBlobAsync(dbPath);
```

