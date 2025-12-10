# Login-System Testanleitung

## ✅ Status: Login-System erfolgreich implementiert

Die Anwendung zeigt jetzt beim Start ein **Login-Fenster** und erlaubt die Anmeldung nur mit gültigen Anmeldedaten.

## Test-Benutzer

**Email:** `test@example.com`
**Passwort:** `password123`

## Test-Szenarios

### 1. Login mit Test-Benutzer
```
1. Starten Sie die Anwendung: dotnet run --project LernApp/LernApp.csproj
2. Es erscheint das Login-Fenster (LoginWindow)
3. Geben Sie ein:
   - Email: test@example.com
   - Passwort: password123
4. Klicken Sie "Anmelden"
5. ✅ Sie sollten automatisch zum Haupt-Fenster (MainWindow) weitergeleitet werden
6. Sie können jetzt Lerneinheiten erstellen/löschen
```

### 2. Registrierung eines neuen Benutzers
```
1. Klicken Sie auf "Registrieren"
2. Geben Sie ein:
   - Email: neuer@example.com
   - Passwort: passwort123 (min. 6 Zeichen)
3. Klicken Sie "Registrieren"
4. ✅ Neue Meldung: "Registrierung erfolgreich! Sie können sich jetzt anmelden."
5. Sie können sich jetzt mit diesem Konto anmelden
```

### 3. Falsche Anmeldedaten
```
1. Geben Sie ein:
   - Email: test@example.com
   - Passwort: falsch
2. Klicken Sie "Anmelden"
3. ✅ Fehlermeldung: "Email oder Passwort ist falsch"
4. Fenster bleibt auf dem Login-Bildschirm
```

### 4. Leere Felder
```
1. Lassen Sie Email oder Passwort leer
2. Klicken Sie "Anmelden"
3. ✅ Fehlermeldung: "Email und Passwort sind erforderlich"
```

## Implementierte Funktionen

### LoginWindow (XAML)
- ✅ Email-Eingabefeld
- ✅ Passwort-Eingabefeld (mit Bullet-Points)
- ✅ Toggle zwischen "Anmelden" und "Registrieren"
- ✅ Fehler-Anzeige
- ✅ Loading-Indikator während Authentifizierung
- ✅ Fluent-Design mit professionellem Aussehen

### LoginViewModel
- ✅ `LoginCommand` - Authentifizierung mit Email/Passwort
- ✅ `RegisterCommand` - Registrierung neuer Benutzer
- ✅ `ToggleRegisterCommand` - Wechsel zwischen Modi
- ✅ `LoginSuccessful` Event - Benachrichtigung bei erfolgreicher Anmeldung
- ✅ Passwort-Validierung (mindestens 6 Zeichen)
- ✅ Fehler-Handling mit aussagekräftigen Meldungen

### App.xaml.cs
- ✅ Zeigt LoginWindow beim Start (nicht MainWindow)
- ✅ `ShowLoginWindow()` - Erstellt LoginViewModel und LoginWindow
- ✅ `ShowMainWindow(int userId)` - Wechselt zum Haupt-Fenster nach Login
- ✅ `OnLoginSuccessful(int userId)` - Event-Handler für Login-Erfolg
- ✅ Lädt automatisch Benutzer-Daten nach erfolgreichem Login

### LernplanViewModel
- ✅ `InitialisiereDatenAsync(int userId)` - Lädt Lerneinheiten des angemeldeten Benutzers

## Architektur

```
LoginWindow.axaml
    ↓
LoginViewModel (ReactiveUI)
    ↓ (on LoginSuccessful event)
App.xaml.cs (ShowMainWindow)
    ↓
LernplanViewModel (mit userId)
    ↓
MainWindow (zeigt Lerneinheiten des Benutzers)
```

## Nächste Schritte

- [ ] Password-Hashing mit BCrypt implementieren (derzeit: Plaintext-Demo)
- [ ] Passwort-Vergessen-Funktion
- [ ] Benutzer-Profil-Seite
- [ ] Session-Verwaltung / Remember-Me
- [ ] Zwei-Faktor-Authentifizierung (optional)

## Sicherheitshinweis

⚠️ **DEMO-MODUS**: Passwörter sind derzeit **unverschlüsselt** gespeichert!
Für Produktionsumgebung: BCrypt oder ähnliche Hashing-Methode implementieren.

