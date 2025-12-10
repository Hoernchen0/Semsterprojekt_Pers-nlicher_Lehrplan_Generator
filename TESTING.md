# Test-Szenarien für LernApp

## Unit Test Beispiele

### Xunit Test-Setup

```bash
cd /workspace
dotnet new xunit -n LernApp.Tests
cd LernApp.Tests
dotnet add reference ../LernApp/LernApp.csproj
```

### Services Tests

```csharp
// LernApp.Tests/Services/UserServiceTests.cs
using Xunit;
using Moq;
using LernApp.Services;
using LernApp.Data.Repositories;
using LernApp.Models;

namespace LernApp.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<ILernAppLogger> _mockLogger;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILernAppLogger>();
        _userService = new UserService(_mockUserRepo.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task RegisteriereBenutzer_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var name = "Test User";
        var email = "test@example.com";
        var passwordHash = "hashedpassword";

        var newUser = new User { Id = 1, Name = name, Email = email, PasswordHash = passwordHash };
        _mockUserRepo.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((User?)null);
        _mockUserRepo.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync(newUser);

        // Act
        var result = await _userService.RegisteriereBenutzerAsync(name, email, passwordHash);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
        _mockUserRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisteriereBenutzer_WithExistingEmail_ShouldThrow()
    {
        // Arrange
        var email = "existing@example.com";
        var existingUser = new User { Email = email };
        _mockUserRepo.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.RegisteriereBenutzerAsync("Test", email, "password")
        );
    }

    [Fact]
    public async Task AuthentifiziereBenutzer_WithCorrectPassword_ShouldReturnUser()
    {
        // Arrange
        var email = "test@example.com";
        var password = "correctpassword";
        var user = new User { Id = 1, Email = email, PasswordHash = password };
        _mockUserRepo.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

        // Act
        var result = await _userService.AuthentifiziereBenutzerAsync(email, password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task AuthentifiziereBenutzer_WithWrongPassword_ShouldReturnNull()
    {
        // Arrange
        var email = "test@example.com";
        var user = new User { Id = 1, Email = email, PasswordHash = "correctpassword" };
        _mockUserRepo.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

        // Act
        var result = await _userService.AuthentifiziereBenutzerAsync(email, "wrongpassword");

        // Assert
        Assert.Null(result);
    }
}
```

### LernplanService Tests

```csharp
// LernApp.Tests/Services/LernplanServiceTests.cs
using Xunit;
using Moq;
using LernApp.Services;
using LernApp.Data.Repositories;
using LernApp.Models;

namespace LernApp.Tests.Services;

public class LernplanServiceTests
{
    private readonly Mock<ILernEinheitRepository> _mockRepo;
    private readonly Mock<ILernAppLogger> _mockLogger;
    private readonly LernplanService _service;

    public LernplanServiceTests()
    {
        _mockRepo = new Mock<ILernEinheitRepository>();
        _mockLogger = new Mock<ILernAppLogger>();
        _service = new LernplanService(_mockRepo.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task ErstelleLernEinheit_ShouldAddAndReturn()
    {
        // Arrange
        var userId = 1;
        var fach = "Mathe";
        var thema = "Integrale";
        
        var expected = new LernEinheit 
        { 
            Id = 1, 
            UserId = userId, 
            Fach = fach, 
            Thema = thema 
        };
        
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<LernEinheit>())).ReturnsAsync(expected);

        // Act
        var result = await _service.ErstelleLernEinheitAsync(userId, fach, thema);

        // Assert
        Assert.Equal(fach, result.Fach);
        Assert.Equal(thema, result.Thema);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<LernEinheit>()), Times.Once);
    }

    [Fact]
    public async Task HoleLernEinheiten_ShouldReturnUserEinheiten()
    {
        // Arrange
        var userId = 1;
        var einheiten = new List<LernEinheit>
        {
            new LernEinheit { Id = 1, UserId = userId, Fach = "Mathe" },
            new LernEinheit { Id = 2, UserId = userId, Fach = "Deutsch" }
        };
        
        _mockRepo.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(einheiten);

        // Act
        var result = await _service.HoleLernEinheitenAsync(userId);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task LöscheLernEinheit_ShouldCallRepository()
    {
        // Arrange
        var einheitId = 1;

        // Act
        await _service.LöscheLernEinheitAsync(einheitId);

        // Assert
        _mockRepo.Verify(r => r.DeleteAsync(einheitId), Times.Once);
    }
}
```

### Repository Tests

```csharp
// LernApp.Tests/Data/UserRepositoryTests.cs
using Xunit;
using Microsoft.EntityFrameworkCore;
using LernApp.Data;
using LernApp.Data.Repositories;
using LernApp.Models;

namespace LernApp.Tests.Data;

public class UserRepositoryTests : IAsyncLifetime
{
    private LernAppDbContext _context = null!;
    private UserRepository _repository = null!;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<LernAppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new LernAppDbContext(options);
        _repository = new UserRepository(_context);
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task AddAsync_ShouldInsertUser()
    {
        // Arrange
        var user = new User 
        { 
            Name = "Test User", 
            Email = "test@example.com",
            PasswordHash = "hashed"
        };

        // Act
        var result = await _repository.AddAsync(user);

        // Assert
        Assert.NotEqual(0, result.Id);
        var savedUser = await _repository.GetByIdAsync(result.Id);
        Assert.NotNull(savedUser);
        Assert.Equal("Test User", savedUser.Name);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldFindUser()
    {
        // Arrange
        var user = new User 
        { 
            Name = "Test", 
            Email = "find@example.com",
            PasswordHash = "hashed"
        };
        await _repository.AddAsync(user);

        // Act
        var result = await _repository.GetByEmailAsync("find@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);
    }

    [Fact]
    public async Task GetByEmailAsync_WithNonExistentEmail_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByEmailAsync("nonexistent@example.com");

        // Assert
        Assert.Null(result);
    }
}
```

## Integration Tests

```csharp
// LernApp.Tests/Integration/LernplanWorkflowTests.cs
using Xunit;
using Microsoft.EntityFrameworkCore;
using LernApp.Data;
using LernApp.Data.Repositories;
using LernApp.Services;

namespace LernApp.Tests.Integration;

public class LernplanWorkflowTests : IAsyncLifetime
{
    private LernAppDbContext _context = null!;
    private IUserService _userService = null!;
    private ILernplanService _lernplanService = null!;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<LernAppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new LernAppDbContext(options);
        
        var logger = new ConsoleLogger();
        var userRepo = new UserRepository(_context);
        var lernRepo = new LernEinheitRepository(_context);

        _userService = new UserService(userRepo, logger);
        _lernplanService = new LernplanService(lernRepo, logger);
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task CompleteWorkflow_CreateUserAndLernEinheit()
    {
        // Arrange & Act
        // 1. Create User
        var user = await _userService.RegisteriereBenutzerAsync(
            "Integration Test User",
            "integration@test.com",
            "password123"
        );

        // 2. Create Lerneinheit
        var einheit = await _lernplanService.ErstelleLernEinheitAsync(
            user.Id,
            "Informatik",
            "Web Development",
            "HTML, CSS, JavaScript"
        );

        // 3. Retrieve Lerneinheiten
        var userEinheiten = await _lernplanService.HoleLernEinheitenAsync(user.Id);

        // Assert
        Assert.NotNull(user);
        Assert.NotEqual(0, user.Id);
        
        Assert.NotNull(einheit);
        Assert.Equal("Informatik", einheit.Fach);
        Assert.Equal("Web Development", einheit.Thema);
        
        Assert.Single(userEinheiten);
        Assert.Equal(einheit.Id, userEinheiten.First().Id);
    }
}
```

## Test Ausführung

```bash
# Alle Tests ausführen
dotnet test

# Nur ein Test-Projekt
dotnet test LernApp.Tests

# Mit Coverage Report
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover

# Spezifischen Test ausführen
dotnet test --filter "FullyQualifiedName~UserServiceTests.RegisteriereBenutzer"
```

## Mock-Setup Best Practices

```csharp
// Setup für häufige Szenarien
_mockRepo.Setup(r => r.AddAsync(It.IsAny<User>()))
    .ReturnsAsync((User u) => 
    {
        u.Id = 1; // Assign ID like DB would
        return u;
    });

// Verify multiple calls
_mockRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.AtLeastOnce);

// Setup with specific arguments
_mockRepo.Setup(r => r.GetByIdAsync(1))
    .ReturnsAsync(new User { Id = 1, Name = "Test" });

// Throw exceptions
_mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
    .ThrowsAsync(new InvalidOperationException("Not found"));
```

---

Diese Tests sichern ab, dass:
- ✅ Services korrekt funktionieren
- ✅ Repositories Datenbank-Operationen korrekt ausführen
- ✅ Workflows end-to-end korrekt sind
- ✅ Error-Handling funktioniert

