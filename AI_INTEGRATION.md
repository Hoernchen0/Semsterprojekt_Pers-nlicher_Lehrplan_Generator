# ai_integration_guide.md

# KI-Integration: Python-Script Kommunikation

## Übersicht

Das System soll mit einem Python-Script kommunizieren, um KI-Funktionen bereitzustellen. Dies erfolgt über Prozessaufrufe.

## Implementierung des AIService

### 1. Python-Script vorbereiten

Erstelle ein Python-Script `ai_script.py` im Projektverzeichnis:

```python
#!/usr/bin/env python3
import sys
import json
from typing import Dict, Any

def generate_learning_plan(topic: str, level: str = "intermediate") -> Dict[str, Any]:
    """
    Generiert einen Lernplan basierend auf dem Thema
    """
    # TODO: Mit OpenAI API, Claude API oder lokalem LLM integrieren
    learning_plan = {
        "topic": topic,
        "level": level,
        "weeks": [
            {
                "week": 1,
                "topics": ["Grundlagen", "Einführung"],
                "tasks": ["Lesen Sie Kapitel 1", "Machen Sie Übung 1"]
            },
            {
                "week": 2,
                "topics": ["Fortgeschrittene Konzepte"],
                "tasks": ["Lesen Sie Kapitel 2", "Lösen Sie Probleme 2"]
            }
        ]
    }
    return learning_plan

def analyze_file(file_content: str) -> Dict[str, Any]:
    """
    Analysiert den Inhalt einer Datei
    """
    # TODO: Mit NLP oder KI-Modell analysieren
    analysis = {
        "summary": "Zusammenfassung des Inhalts",
        "key_points": ["Punkt 1", "Punkt 2", "Punkt 3"],
        "difficulty": "intermediate",
        "estimated_learning_time": 120  # Minuten
    }
    return analysis

def main():
    if len(sys.argv) < 2:
        print(json.dumps({"error": "Nicht genug Argumente"}))
        sys.exit(1)
    
    command = sys.argv[1]
    
    try:
        if command == "generate_plan":
            topic = sys.argv[2] if len(sys.argv) > 2 else "General Topic"
            level = sys.argv[3] if len(sys.argv) > 3 else "intermediate"
            result = generate_learning_plan(topic, level)
            print(json.dumps(result, ensure_ascii=False, indent=2))
        
        elif command == "analyze_file":
            # Lesen Sie den Dateiinhalt von stdin
            file_content = sys.stdin.read()
            result = analyze_file(file_content)
            print(json.dumps(result, ensure_ascii=False, indent=2))
        
        else:
            print(json.dumps({"error": f"Unbekannter Befehl: {command}"}))
            sys.exit(1)
    
    except Exception as e:
        print(json.dumps({"error": str(e)}))
        sys.exit(1)

if __name__ == "__main__":
    main()
```

### 2. C# Service für Python-Kommunikation

Aktualisieren Sie die `AIService.cs`:

```csharp
public async Task<string> RufeAIPythonScriptAsync(string prompt)
{
    try
    {
        using (var process = new System.Diagnostics.Process())
        {
            process.StartInfo.FileName = "python3"; // oder "python" auf Windows
            process.StartInfo.Arguments = $"ai_script.py generate_plan \"{prompt}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                _logger.LogError($"Python-Script Fehler: {error}");
                throw new InvalidOperationException($"Python-Script fehlgeschlagen: {error}");
            }

            var result = JsonSerializer.Deserialize<JsonElement>(output);
            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        }
    }
    catch (Exception ex)
    {
        _logger.LogError($"Fehler beim Aufruf des Python Scripts: {ex.Message}");
        throw;
    }
}
```

### 3. Mit externen KI-APIs integrieren

#### OpenAI Integration:

```python
import openai

def generate_learning_plan_openai(topic: str) -> Dict[str, Any]:
    openai.api_key = os.getenv("OPENAI_API_KEY")
    
    response = openai.ChatCompletion.create(
        model="gpt-4",
        messages=[
            {"role": "system", "content": "Du bist ein Bildungsexperte."},
            {"role": "user", "content": f"Erstelle einen Lernplan für: {topic}"}
        ]
    )
    
    return {
        "topic": topic,
        "plan": response.choices[0].message.content
    }
```

#### Anthropic Claude Integration:

```python
import anthropic

def generate_learning_plan_claude(topic: str) -> Dict[str, Any]:
    client = anthropic.Anthropic(api_key=os.getenv("ANTHROPIC_API_KEY"))
    
    message = client.messages.create(
        model="claude-3-opus-20240229",
        max_tokens=1024,
        messages=[
            {"role": "user", "content": f"Erstelle einen Lernplan für: {topic}"}
        ]
    )
    
    return {
        "topic": topic,
        "plan": message.content[0].text
    }
```

## Deployment-Überlegungen

### Lokal (Desktop)
- Python 3.8+ muss installiert sein
- Script im gleichen Verzeichnis wie die Anwendung
- Funktioniert auf Windows, macOS, Linux

### Web (ASP.NET Core)
- Python auf dem Server installiert
- Container-basiertes Deployment mit Docker
- Oder Azure Functions für serverlose KI

### Docker-Beispiel

```dockerfile
FROM mcr.microsoft.com/dotnet/runtime:8.0

# Python installieren
RUN apt-get update && apt-get install -y python3 python3-pip

# Dependencies installieren
COPY requirements.txt .
RUN pip3 install -r requirements.txt

# App kopieren
COPY . .

ENTRYPOINT ["dotnet", "LernApp.dll"]
```

## Sicherheit

1. **Input Validation**: Prompt validieren vor Übergabe
2. **Timeout**: Process mit Timeout versehen
3. **Error Handling**: Fehler abfangen und loggen
4. **Rate Limiting**: Zu häufige API-Aufrufe limitieren

## Performance

- Prompts asynchron verarbeiten
- Ergebnisse cachen wo möglich
- Batch-Verarbeitung für mehrere Prompts
- Optional: Hintergrund-Worker (Hangfire, Quartz)

