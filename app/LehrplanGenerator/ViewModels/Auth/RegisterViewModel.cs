using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.Logic.Utils;
using LehrplanGenerator.ViewModels.Shell;

namespace LehrplanGenerator.ViewModels.Auth;

public partial class RegisterViewModel : ViewModelBase
{
    [ObservableProperty] private string firstName = string.Empty;
    [ObservableProperty] private string lastName = string.Empty;
    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string? errorMessage;

    private readonly UserCredentialStore _store;
    private readonly INavigationService _navigationService;
    private readonly AppState _appState;

    public RegisterViewModel(UserCredentialStore store, INavigationService navigationService, AppState appState)
    {
        _store = store;
        _navigationService = navigationService;
        _appState = appState;
    }

    [RelayCommand]
    private void Register()
    {
        try
        {
            Console.WriteLine("=== Registrierung Start ===");
            
            ErrorMessage = ValidateInput();
            if (ErrorMessage != null)
            {
                Console.WriteLine($"❌ Validierung fehlgeschlagen: {ErrorMessage}");
                return;
            }

            Console.WriteLine("✓ Validierung erfolgreich");

            var userId = Guid.NewGuid();
            Console.WriteLine($"✓ UserID erstellt: {userId}");

            var user = new UserCredential(
                userId,
                FirstName,
                LastName,
                Username,
                PasswordHasher.Hash(Password)
            );
            
            Console.WriteLine($"✓ UserCredential erstellt");

            _store.Add(user);
            Console.WriteLine($"✓ Benutzer in Store hinzugefügt");

            _appState.CurrentUserId = user.UserId;
            _appState.CurrentUserDisplayName = $"{user.FirstName} {user.LastName}";
            
            Console.WriteLine($"✓ AppState aktualisiert");

            Console.WriteLine($"✓ Benutzer {Username} erfolgreich registriert");

            _navigationService.NavigateTo<ShellViewModel>();
            Console.WriteLine("✓ Navigation zu ShellViewModel erfolgreich");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ FEHLER bei Registrierung: {ex.GetType().Name}");
            Console.WriteLine($"❌ Nachricht: {ex.Message}");
            Console.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
            
            ErrorMessage = $"Fehler bei der Registrierung: {ex.Message}";
        }
    }

    [RelayCommand]
    private void Menu()
    {
        _navigationService.NavigateTo<LehrplanGenerator.ViewModels.Main.MainViewModel>();
    }

    private string? ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(FirstName) || FirstName.Length < 2)
            return "Vorname muss mindestens 2 Zeichen lang sein.";

        if (string.IsNullOrWhiteSpace(LastName) || LastName.Length < 2)
            return "Nachname muss mindestens 2 Zeichen lang sein.";

        if (string.IsNullOrWhiteSpace(Username) || !Username.All(c => char.IsLetterOrDigit(c) || c == '_'))
            return "Username darf nur Buchstaben, Zahlen und Unterstriche enthalten.";

        if (_store.UsernameExists(Username))
            return "Username ist bereits vergeben.";

        if (string.IsNullOrWhiteSpace(Password) || Password.Length < 8
            || !Password.Any(char.IsUpper)
            || !Password.Any(char.IsLower)
            || !Password.Any(char.IsDigit)
            || !Password.Any(c => !char.IsLetterOrDigit(c)))
            return "Passwort: mindestens 8 Zeichen, Groß-/Kleinbuchstabe, Zahl und Sonderzeichen.";

        return null;
    }
}
