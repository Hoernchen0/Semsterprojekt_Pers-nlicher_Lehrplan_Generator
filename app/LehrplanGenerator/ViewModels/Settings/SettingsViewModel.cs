using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.Logic.Utils;
using LehrplanGenerator.ViewModels.Main;
using LehrplanGenerator.Views.Settings;
using System;
using System.Threading.Tasks;

namespace LehrplanGenerator.ViewModels.Settings;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly AppState _appState;
    private readonly UserCredentialStore _store;
    private readonly ChatBufferService _chatBufferService;
    private readonly ExportService _exportService;

    public string? CurrentUserDisplayName => _appState.CurrentUserDisplayName;
    public string? CurrentUsername => _appState.CurrentUsername;


    public SettingsViewModel(
        INavigationService navigationService,
        AppState appState,
        UserCredentialStore store,
        ChatBufferService chatBufferService,
        ExportService exportService)
    {
        _navigationService = navigationService;
        _appState = appState;
        _store = store;
        _chatBufferService = chatBufferService;
        _exportService = exportService;

        ThemeManager.Instance.PropertyChanged += (_, __) =>
        {
            OnPropertyChanged(nameof(ThemeButtonText));
        };
    }


    // =====================
    // THEME
    // =====================

    public string ThemeButtonText =>
        ThemeManager.Instance.IsDark
            ? "Dark Mode aktiv"
            : "Light Mode aktiv";

    [RelayCommand]
    private void ToggleTheme()
    {
        ThemeManager.Instance.Toggle();
    }

    [RelayCommand]
    private void EditProfile()
    {
        if (_appState.CurrentUsername == null)
            return;

        var lifetime = Avalonia.Application.Current?.ApplicationLifetime
            as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime;

        var mainWindow = lifetime?.MainWindow;

        UsernameModificationView? window = null;

        var vm = new UsernameModificationViewModel(
            _appState.CurrentUsername,
            _store,
            _appState,
            result =>
            {
                if (result != null)
                {
                    OnPropertyChanged(nameof(CurrentUserDisplayName));
                    OnPropertyChanged(nameof(CurrentUsername));
                }

                window?.Close();
            });

        window = new UsernameModificationView
        {
            DataContext = vm
        };

        if (mainWindow != null)
        {
            window.ShowDialog(mainWindow);
        }
    }



    [RelayCommand]
    private void EditPassword()
    {
        if (_appState.CurrentUsername == null)
            return;

        var lifetime = Avalonia.Application.Current?.ApplicationLifetime
            as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime;

        var mainWindow = lifetime?.MainWindow;

        PasswordModificationView? window = null;

        var vm = new PasswordModificationViewModel(
            _appState.CurrentUsername,
            _store,
            _appState,
            _ => window?.Close()
        );

        window = new PasswordModificationView
        {
            DataContext = vm
        };

        if (mainWindow != null)
        {
            window.ShowDialog(mainWindow);
        }
    }






    // =====================
    // ACCOUNT
    // =====================

    [RelayCommand]
    private void LogOut()
    {
        _appState.CurrentUserId = null;
        _appState.CurrentUserDisplayName = null;

        _navigationService.NavigateTo<MainViewModel>();
    }

    // =====================
    // CHAT BUFFER
    // =====================

    [RelayCommand]
    private void ClearChatBuffer()
    {
        _chatBufferService.ClearChatBuffer();
        Console.WriteLine($"✓ Chat-Buffer gelöscht ({_chatBufferService.GetBufferMessageCount()} Nachrichten)");
    }

    // =====================
    // EXPORT DATEN
    // =====================

    [RelayCommand]
    private async Task ExportDataAsync()
    {
        if (_appState.CurrentUserId == null)
        {
            Console.WriteLine("❌ Kein Benutzer angemeldet!");
            return;
        }

        try
        {
            var filePath = await _exportService.SaveExportToFileAsync(_appState.CurrentUserId.Value);
            Console.WriteLine($"✓ Export erfolgreich: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Fehler beim Export: {ex.Message}");
        }
    }
}
