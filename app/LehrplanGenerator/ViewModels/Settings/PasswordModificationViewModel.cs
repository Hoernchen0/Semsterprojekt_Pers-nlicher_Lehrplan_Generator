using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;

namespace LehrplanGenerator.ViewModels.Settings;

public partial class PasswordModificationViewModel : ViewModelBase
{
    private readonly UserCredentialStore _store;
    private readonly Action _close;

    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string newPassword = string.Empty;
    [ObservableProperty] private string? errorMessage;

    public PasswordModificationViewModel(
        string currentUsername,
        UserCredentialStore store,
        AppState appState,
        Action<string?> close)
    {
        Username = currentUsername;
        _store = store;
        _close = () => close(null);
    }

    [RelayCommand]
    private void EditPassword()
    {
        if (string.IsNullOrWhiteSpace(NewPassword))
        {
            ErrorMessage = "Passwort darf nicht leer sein.";
            return;
        }

        try
        {
            _store.UpdatePassword(Username, NewPassword);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return;
        }

        _close();
    }

    [RelayCommand]
    private void Cancel() => _close();
}
