# Script um zu überprüfen ob die SQLite-Datenbank korrekt verwendet wird

Write-Host "=== Datenbank Überprüfung ===" -ForegroundColor Cyan

# 1. Überprüfe ob DB-Dateien existieren
Write-Host "`n1. Überprüfe SQLite-Dateien im AppData Ordner:" -ForegroundColor Yellow
$appDataPath = "$env:APPDATA\LehrplanGenerator"
if (Test-Path $appDataPath) {
    $dbFiles = Get-ChildItem $appDataPath -Filter "*.db" -Force
    if ($dbFiles) {
        Write-Host "✅ SQLite Dateien gefunden:" -ForegroundColor Green
        $dbFiles | ForEach-Object {
            Write-Host "   - $($_.Name) ($('{0:N0}' -f $_.Length) bytes, modifiziert: $($_.LastWriteTime))"
        }
    } else {
        Write-Host "❌ Keine SQLite Dateien gefunden!" -ForegroundColor Red
    }
} else {
    Write-Host "❌ AppData/LehrplanGenerator Ordner existiert nicht!" -ForegroundColor Red
}

# 2. Überprüfe alte JSON-Dateien
Write-Host "`n2. Überprüfe auf alte JSON-Dateien:" -ForegroundColor Yellow
$jsonFiles = Get-ChildItem "c:\1 hfu\6_sem\Semesterprojekt\Semsterprojekt_Pers-nlicher_Lehrplan_Generator" -Recurse -Filter "*datenbank*.json" -Force
if ($jsonFiles) {
    Write-Host "⚠️  JSON Dateien gefunden (sollten nicht verwendet werden):" -ForegroundColor Yellow
    $jsonFiles | ForEach-Object {
        Write-Host "   - $($_.FullName)"
    }
} else {
    Write-Host "✅ Keine alten JSON-Datenbank Dateien gefunden" -ForegroundColor Green
}

# 3. Überprüfe ob SQLite Tools verfügbar sind
Write-Host "`n3. Überprüfe SQLite3 Verfügbarkeit:" -ForegroundColor Yellow
try {
    $sqliteVersion = sqlite3 --version 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ SQLite3 CLI verfügbar: $sqliteVersion" -ForegroundColor Green
        
        # Wenn DB existiert, zeige Tabellen
        if (Test-Path "$appDataPath\lernapp.db") {
            Write-Host "`n4. Tabellen in SQLite Datenbank:" -ForegroundColor Yellow
            $tables = sqlite3 "$appDataPath\lernapp.db" ".tables" 2>&1
            if ($tables) {
                Write-Host "   $tables" -ForegroundColor Green
            }
            
            # Zeige User-Tabelle Struktur
            Write-Host "`n5. Benutzer-Tabelle Struktur:" -ForegroundColor Yellow
            sqlite3 "$appDataPath\lernapp.db" ".schema" 2>&1 | Select-Object -First 20
        }
    }
} catch {
    Write-Host "⚠️  SQLite3 CLI nicht verfügbar oder nicht installiert" -ForegroundColor Yellow
    Write-Host "   Du kannst trotzdem .NET verwendet sein lassen" -ForegroundColor Gray
}

# 4. Zusammenfassung
Write-Host "`n=== Zusammenfassung ===" -ForegroundColor Cyan
Write-Host "✅ Wenn du SQLite DB-Dateien siehst → die App verwendet SQLite3" -ForegroundColor Green
Write-Host "❌ Wenn nur JSON-Dateien im Projekt → die App verwendet noch JSON" -ForegroundColor Red
Write-Host "✅ Die Datenbank sollte im AppData/Roaming/LehrplanGenerator liegen" -ForegroundColor Green
