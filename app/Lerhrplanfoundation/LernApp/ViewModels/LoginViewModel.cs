using System;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using Avalonia.Threading;
using LernApp.Services;

namespace LernApp.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly IUserService _userService;
    private readonly ILernAppLogger _logger;

    private string _email = "";
    private string _password = "";
    private string _errorMessage = "";
    private bool _isLoading;
    private bool _showRegister;

    public ReactiveCommand<Unit, Unit> LoginCommand { get; }
    public ReactiveCommand<Unit, Unit> RegisterCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleRegisterCommand { get; }

    public event Action<int>? LoginSuccessful;

    public LoginViewModel(IUserService userService, ILernAppLogger logger)
    {
        _userService = userService;
        _logger = logger;

        LoginCommand = ReactiveCommand.CreateFromTask(ExecuteLogin, outputScheduler: RxApp.MainThreadScheduler);
        RegisterCommand = ReactiveCommand.CreateFromTask(ExecuteRegister, outputScheduler: RxApp.MainThreadScheduler);
        ToggleRegisterCommand = ReactiveCommand.Create(ToggleRegister);
    }

    public string Email
    {
        get => _email;
        set => this.RaiseAndSetIfChanged(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    public bool ShowRegister
    {
        get => _showRegister;
        set => this.RaiseAndSetIfChanged(ref _showRegister, value);
    }

    private void ToggleRegister()
    {
        ShowRegister = !ShowRegister;
        ErrorMessage = "";
    }

    private async Task ExecuteLogin()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = "";

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Email und Passwort sind erforderlich";
                _logger.LogWarning("Login-Versuch: Leere Felder");
                return;
            }

            _logger.LogInfo($"🔐 Login-Versuch für: {Email}");
            
            var user = await _userService.AuthentifiziereBenutzerAsync(Email, Password);
            if (user == null)
            {
                ErrorMessage = "Email oder Passwort ist falsch";
                _logger.LogWarning($"❌ Login FEHLGESCHLAGEN für: {Email}");
                _logger.LogWarning($"   Passwort eingegeben: {Password}");
                return;
            }

            _logger.LogInfo($"✅ Benutzer angemeldet: {user.Email} (ID: {user.Id})");
            
            // Rufe Event direkt auf (nicht await!)
            _logger.LogInfo($"🔄 Event 'LoginSuccessful' wird mit userId={user.Id} aufgerufen");
            LoginSuccessful?.Invoke(user.Id);
            _logger.LogInfo($"✅ Event 'LoginSuccessful' wurde aufgerufen");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Login-Fehler: {ex.Message}";
            _logger.LogError($"❌ {ErrorMessage}\n{ex.StackTrace}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ExecuteRegister()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = "";

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Email und Passwort sind erforderlich";
                return;
            }

            if (Password.Length < 6)
            {
                ErrorMessage = "Passwort muss mindestens 6 Zeichen lang sein";
                return;
            }

            // Extrahiere Namen aus Email
            var namePart = Email.Split('@')[0];

            var user = await _userService.RegisteriereBenutzerAsync(namePart, Email, Password);

            _logger.LogInfo($"✅ Benutzer registriert: {user.Email} (ID: {user.Id})");
            
            // Kleine Verzögerung für Datenbank-Konsistenz
            await Task.Delay(1000);
            
            // Überprüfe sofort, ob Benutzer existiert
            var verifyUser = await _userService.AuthentifiziereBenutzerAsync(user.Email, Password);
            if (verifyUser != null)
            {
                _logger.LogInfo($"✅ Verifizierung erfolgreich: {verifyUser.Email} mit ID {verifyUser.Id}");
            }
            else
            {
                _logger.LogWarning($"⚠️ Verifizierung FEHLGESCHLAGEN: {user.Email}");
                ErrorMessage = "⚠️ Benutzer wurde erstellt, aber Login schlägt fehl. Bitte neue Datenbank-Datei löschen und App neustarten!";
                ShowRegister = false;
                Email = "";
                Password = "";
                return;
            }

            ErrorMessage = "Registrierung erfolgreich! Sie können sich jetzt anmelden.";
            ShowRegister = false;
            Email = "";
            Password = "";
        }
        catch (InvalidOperationException ex)
        {
            ErrorMessage = $"Fehler: {ex.Message}";
            _logger.LogError($"❌ {ErrorMessage}");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Registrierungs-Fehler: {ex.Message}";
            _logger.LogError($"❌ {ErrorMessage}");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
