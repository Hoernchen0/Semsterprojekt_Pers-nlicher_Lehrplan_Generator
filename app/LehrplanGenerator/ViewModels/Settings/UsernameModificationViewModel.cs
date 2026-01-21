using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;

namespace LehrplanGenerator.ViewModels.Settings;

public partial class UsernameModificationViewModel : ViewModelBase
{
    private readonly UserCredentialStore _store;
    private readonly AppState _appState;
    private readonly Action<string?> _close;

    [ObservableProperty] private string firstName = string.Empty;
    [ObservableProperty] private string lastName = string.Empty;
    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string password = string.Empty;

    [ObservableProperty] private string newusername = string.Empty;

    [ObservableProperty] private string? errorMessage;

    public UsernameModificationViewModel(
        string currentUsername,
        UserCredentialStore store,
        AppState appState,
        Action<string?> close)
    {
        Username = currentUsername;
        _store = store;
        _appState = appState;
        _close = close;
    }

    [RelayCommand]
    private void EditUsername()
    {
        ErrorMessage = ValidateUsername();
        if (ErrorMessage != null)
            return;

        try
        {
            _store.UpdateUsername(Username, Newusername);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Fehler beim Speichern: " + ex.Message;
            return;
        }

        _appState.CurrentUsername = Newusername;
        _close(Newusername);
    }



    private string? ValidateUsername()
    {
        if (string.IsNullOrWhiteSpace(Newusername) ||
            !Newusername.All(c => char.IsLetterOrDigit(c) || c == '_'))
            return "Username darf nur Buchstaben, Zahlen und Unterstriche enthalten.";

        if (Newusername == Username)
            return "Der neue Benutzername ist identisch mit dem alten.";

        if (_store.UsernameExists(Newusername))
            return "Username ist bereits vergeben.";

        return null;
    }




    [RelayCommand]
    private void Cancel()
    {
        _close(null);
    }
}
