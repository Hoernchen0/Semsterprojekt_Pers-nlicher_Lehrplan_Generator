using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Models;
using UserAlias = LehrplanGenerator.Logic.Models.User;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.Logic.Utils;

using LehrplanGenerator.Views.Windows;
using LehrplanGenerator.Views.Shell;

namespace LehrplanGenerator.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string password = string.Empty;

    [ObservableProperty] private string result = string.Empty;

    private readonly MainWindow _mainWindow;
    private readonly UserCredentialStore _store;

    public LoginViewModel()
    {
        _store = App.CredentialStore;
    }

    public LoginViewModel(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        _store = App.CredentialStore;
    }

    [RelayCommand]
    private void Login()
    {
        var cred = _store.GetAll().FirstOrDefault(c => c.Username == Username);

        if (cred is null)
        {
            Result = "falsch";
            return;
        }

        var hashed = PasswordHasher.Hash(Password);

        if (hashed == cred.PasswordHash)
        {
            LehrplanGenerator.Logic.State.AppState.CurrentUser = new LehrplanGenerator.Logic.Models.User(cred.UserId, cred.Username);
            Result = "richtig";


            _mainWindow.RootContent.Content = new ShellView(_mainWindow);
        }
        else
        {
            Result = "falsch";
        }
    }

}
