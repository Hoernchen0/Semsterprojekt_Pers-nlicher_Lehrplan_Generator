using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Models;
using UserAlias = LehrplanGenerator.Logic.Models.User;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using Microsoft.Extensions.DependencyInjection;
using LehrplanGenerator.Logic.Utils;

using LehrplanGenerator.Views.Windows;
using LehrplanGenerator.Views.Shell;

namespace LehrplanGenerator.ViewModels;

public partial class LegacyLoginViewModel : ViewModelBase
{
    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string password = string.Empty;

    [ObservableProperty] private string result = string.Empty;

    private readonly MainWindow? _mainWindow;
    private readonly UserCredentialStore _store;

    public LegacyLoginViewModel()
    {
        _store = App.Services.GetRequiredService<UserCredentialStore>();
    }

    public LegacyLoginViewModel(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        _store = App.Services.GetRequiredService<UserCredentialStore>();
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
            LehrplanGenerator.Logic.State.AppState.CurrentUser = new UserAlias(cred.UserId, cred.FirstName, cred.LastName);
            Result = "richtig";

            if (_mainWindow is not null)
            {
                _mainWindow.Content = new ShellView();
            }
        }
        else
        {
            Result = "falsch";
        }
    }

}
