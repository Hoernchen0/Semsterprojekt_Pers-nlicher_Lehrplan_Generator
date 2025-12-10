# Web/ASP.NET Core Integration

Wenn Sie das System auch als Web-App deployen möchten, erstellen Sie ein neues ASP.NET Core Projekt:

```bash
cd /workspace
dotnet new webapi -n LernApp.Web
```

## Shared Project für Services und Models

Um Code zu teilen zwischen Desktop (Avalonia) und Web (ASP.NET), erstellen Sie ein Shared-Projekt:

```bash
dotnet new classlib -n LernApp.Shared
```

### Projektstruktur

```
/workspace
├── LernApp.Shared/              (Class Library)
│   ├── Models/
│   ├── Services/
│   ├── Data/
│   └── LernApp.Shared.csproj
├── LernApp/                     (Avalonia Desktop)
│   └── LernApp.csproj
└── LernApp.Web/                 (ASP.NET Core Web API)
    └── LernApp.Web.csproj
```

### LernApp.Web/Program.cs

```csharp
using Microsoft.EntityFrameworkCore;
using LernApp.Data;
using LernApp.Data.Repositories;
using LernApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
string dbPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
    "lernapp-web.db");

builder.Services.AddDbContext<LernAppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// Repositories
builder.Services.AddScoped<IRepository<DateiAnalyse>, Repository<DateiAnalyse>>();
builder.Services.AddScoped<IRepository<GenerierteCSV>, Repository<GenerierteCSV>>();
builder.Services.AddScoped<IRepository<Prompt>, Repository<Prompt>>();
builder.Services.AddScoped<IRepository<UserEinstellung>, Repository<UserEinstellung>>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILernEinheitRepository, LernEinheitRepository>();
builder.Services.AddScoped<IPromptRepository, PromptRepository>();
builder.Services.AddScoped<IGenerierteCSVRepository, GenerierteCSVRepository>();

// Services
builder.Services.AddScoped<ILernAppLogger, ConsoleLogger>();
builder.Services.AddScoped<ILernplanService, LernplanService>();
builder.Services.AddScoped<IAIService, AIService>();
builder.Services.AddScoped<IDateiAnalyseService, DateiAnalyseService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserAppSettingsService, UserAppSettingsService>();

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Datenbank initialisieren
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LernAppDbContext>();
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

## API-Endpoints-Beispiele

### LernEinheit Controller

```csharp
using Microsoft.AspNetCore.Mvc;
using LernApp.Models;
using LernApp.Services;

namespace LernApp.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LernEinheitenController : ControllerBase
{
    private readonly ILernplanService _lernplanService;

    public LernEinheitenController(ILernplanService lernplanService)
    {
        _lernplanService = lernplanService;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<LernEinheit>>> GetByUser(int userId)
    {
        var einheiten = await _lernplanService.HoleLernEinheitenAsync(userId);
        return Ok(einheiten);
    }

    [HttpPost]
    public async Task<ActionResult<LernEinheit>> Create(CreateLernEinheitDto dto)
    {
        var einheit = await _lernplanService.ErstelleLernEinheitAsync(
            dto.UserId,
            dto.Fach,
            dto.Thema,
            dto.Beschreibung);
        return CreatedAtAction(nameof(Create), new { id = einheit.Id }, einheit);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _lernplanService.LöscheLernEinheitAsync(id);
        return NoContent();
    }
}

public class CreateLernEinheitDto
{
    public int UserId { get; set; }
    public string Fach { get; set; } = "";
    public string Thema { get; set; } = "";
    public string? Beschreibung { get; set; }
}
```

### User Authentication Controller

```csharp
using Microsoft.AspNetCore.Mvc;
using LernApp.Services;

namespace LernApp.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = await _userService.RegisteriereBenutzerAsync(
            dto.Name,
            dto.Email,
            dto.PasswordHash);
        return Ok(new { userId = user.Id, email = user.Email });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _userService.AuthentifiziereBenutzerAsync(dto.Email, dto.PasswordHash);
        if (user == null)
            return Unauthorized();
        
        return Ok(new { userId = user.Id, email = user.Email });
    }
}

public class RegisterDto
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
}

public class LoginDto
{
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
}
```

## Docker für Web-Deployment

### Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["LernApp.Web/LernApp.Web.csproj", "LernApp.Web/"]
COPY ["LernApp.Shared/LernApp.Shared.csproj", "LernApp.Shared/"]
RUN dotnet restore "LernApp.Web/LernApp.Web.csproj"

COPY . .
WORKDIR "/src/LernApp.Web"
RUN dotnet build "LernApp.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LernApp.Web.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "LernApp.Web.dll"]
```

### docker-compose.yml

```yaml
version: '3.8'

services:
  web:
    build: .
    ports:
      - "8080:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    volumes:
      - ./data:/app/data
    depends_on:
      - db

  db:
    image: sqlite:latest
    volumes:
      - ./db:/var/lib/sqlite
```

## Frontend-Integration (React/Vue/Angular)

```javascript
// Beispiel für Axios-Client
const API_URL = 'http://localhost:8080/api';

class LernAppClient {
  async getLernEinheiten(userId) {
    const response = await axios.get(`${API_URL}/lerneinheiten/${userId}`);
    return response.data;
  }

  async createLernEinheit(userId, fach, thema, beschreibung) {
    const response = await axios.post(`${API_URL}/lerneinheiten`, {
      userId,
      fach,
      thema,
      beschreibung
    });
    return response.data;
  }

  async registerUser(name, email, passwordHash) {
    const response = await axios.post(`${API_URL}/auth/register`, {
      name,
      email,
      passwordHash
    });
    return response.data;
  }
}
```

