using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.Utils;
using LehrplanGenerator.Views.Main;
using LehrplanGenerator.Views.Windows;

namespace LehrplanGenerator.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string password = string.Empty;

    private readonly MainWindow _mainWindow;
    private readonly UserCredentialStore _store;

    public RegisterViewModel()
    {
        _store = App.CredentialStore;
    }

    public RegisterViewModel(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        _store = App.CredentialStore;
    }

    [RelayCommand]
    private void Register()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            return;

        if (_store.UsernameExists(Username))
            return;

        var cred = new UserCredential(
            Guid.NewGuid(),
            Username,
            PasswordHasher.Hash(Password)
        );

        _store.Add(cred);


        _mainWindow.RootContent.Content = new MainView(_mainWindow);
    }

    [RelayCommand]
    private void Menu()
    {
        _mainWindow.RootContent.Content = new MainView(_mainWindow);
    }
}
