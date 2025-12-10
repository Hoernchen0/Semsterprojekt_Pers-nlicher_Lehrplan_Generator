# 🧪 Datenbank-Integration & Login-Test

## Problem gefunden & behoben

Das Problem war, dass die alte Datenbank zwischen Tests nicht gelöscht wurde und zu Datenkonsistenz-Problemen führte.

**Lösung:** 
- `dbContext.Database.EnsureDeleted()` hinzugefügt (DEV-Modus)
- 1 Sekunde Verzögerung nach Registrierung hinzugefügt
- Besseres Logging für Fehlersuche

## Test durchführen

### Voraussetzung: Alte Datenbank löschen
```bash
rm ~/.local/share/lernapp.db  # Linux
# oder
del "%APPDATA%\lernapp.db"  # Windows
```

### Schritt 1: App starten
```bash
dotnet run --project LernApp/LernApp.csproj
```

### Schritt 2: Registrieren
1. Klick auf "Registrieren" Tab
2. Email: **neuer@test.de**
3. Passwort: **passwort123** (min. 6 Zeichen)
4. Klick "Registrieren"
5. ✅ Sie sehen: "Registrierung erfolgreich! Sie können sich jetzt anmelden."

### Schritt 3: Login mit neuem Benutzer
1. Klick auf "Anmelden" Tab
2. Email: **neuer@test.de**
3. Passwort: **passwort123**
4. Klick "Anmelden"
5. ✅ Sie sollten zum MainWindow weitergeleitet werden

## Wenn es nicht funktioniert

**Überprüfe die Console-Ausgabe auf diese Messages:**

```
✅ Test-Benutzer erstellt: test@example.com / password123
🗑️  Alte Datenbank gelöscht (DEV-MODUS)
✅ Datenbank erstellt

🔐 Login-Versuch für: neuer@test.de
✅ Benutzer angemeldet: neuer@test.de (ID: 2)
```

### Debugging-Tipps

1. **"Benutzer existiert bereits"** → Datenbank nicht gelöscht, alte Registrierung noch da
   - Löschen: `rm ~/.local/share/lernapp.db`
   
2. **"Email oder Passwort ist falsch"** → Benutzer nicht in DB gespeichert
   - Prüfe ob "✅ Benutzer registriert" in Console steht
   - Prüfe ob DB-Datei existiert und größer als 8KB ist
   
3. **Fenster bleibt auf Login-Screen** → Event nicht gefeuert
   - Prüfe ob "✅ Benutzer angemeldet" in Console steht

## Datenbank-Pfade

| OS | Pfad |
|---|---|
| Linux | `~/.local/share/lernapp.db` |
| Windows | `%APPDATA%\lernapp.db` |
| macOS | `~/Library/Application Support/lernapp.db` |

## Implementierte Fixes

✅ **DbContext.EnsureDeleted()** - Alte Datenbank im DEV-Modus löschen
✅ **1 Sekunde Verzögerung** - Nach Registrierung für Datenbank-Konsistenz  
✅ **Verifizierungs-Login** - Nach Registrierung prüfen, ob Benutzer existiert
✅ **Besseres Logging** - Mit ✅ und ❌ Symbolen für einfacheres Debugging
✅ **Fehlermeldung** - Wenn Verifizierung fehlschlägt, aussagekräftige Fehlermeldung anzeigen

## Nächste Schritte (TODO)

- [ ] Production-Modus: `EnsureDeleted()` entfernen
- [ ] Password-Hashing mit BCrypt implementieren
- [ ] Migrations-System für Schema-Änderungen
- [ ] Datenbank-Backup vor jedem Update
