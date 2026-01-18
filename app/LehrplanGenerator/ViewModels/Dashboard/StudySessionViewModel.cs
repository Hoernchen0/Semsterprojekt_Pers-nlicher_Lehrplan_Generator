using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Logic.Services;
using Avalonia.Threading;
using System;
using System.Threading.Tasks;
using LehrplanGenerator.ViewModels.Shell;

namespace LehrplanGenerator.ViewModels.Dashboard;

public partial class StudySessionViewModel : ViewModelBase
{
    private readonly LearningProgressService _learningProgressService;
    private readonly INavigationService _navigationService;

    private LearningProgressEntity _unit = null!;
    private DispatcherTimer _timer = null!;
    private TimeSpan _remaining;

    // =====================
    // Bindings
    // =====================

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string remainingTimeText = "00:00";

    [ObservableProperty]
    private bool isRunning;

    [ObservableProperty]
    private bool isPaused;

    // =====================
    // Constructor
    // =====================

    public StudySessionViewModel(
        LearningProgressService learningProgressService,
        INavigationService navigationService)
    {
        _learningProgressService = learningProgressService;
        _navigationService = navigationService;
    }

    // =====================
    // Init
    // =====================

    public void Init(LearningProgressEntity unit)
    {
        _unit = unit;

        Title = $"{_unit.Subject} – {_unit.ModuleName}";

        // 🔑 NUR Dauer, Datum egal
        _remaining =
            _unit.PlannedEnd.TimeOfDay -
            _unit.PlannedStart.TimeOfDay;

        if (_remaining < TimeSpan.Zero)
            _remaining = TimeSpan.Zero;

        UpdateText();

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += (_, __) => Tick();

        Start();
    }

    // =====================
    // Timer
    // =====================

    private void Tick()
    {
        if (_remaining <= TimeSpan.Zero)
        {
            _ = FinishAsync();
            return;
        }

        _remaining = _remaining.Subtract(TimeSpan.FromSeconds(1));
        UpdateText();
    }

    private void UpdateText()
    {
        RemainingTimeText =
            $"{(int)_remaining.TotalMinutes:D2}:{_remaining.Seconds:D2}";
    }

    // =====================
    // Commands
    // =====================

    [RelayCommand]
    private void Start()
    {
        IsRunning = true;
        IsPaused = false;
        _timer.Start();
    }

    [RelayCommand]
    private void Pause()
    {
        IsPaused = true;
        IsRunning = false;
        _timer.Stop();
    }

    [RelayCommand]
    private async Task FinishAsync()
    {
        _timer.Stop();

        await _learningProgressService.MarkCompletedAsync(_unit.Id);

        _navigationService.NavigateTo<ShellViewModel>();
    }
}
