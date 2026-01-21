using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Threading;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.ViewModels.Shell;
using System;
using System.Threading.Tasks;

namespace LehrplanGenerator.ViewModels.Dashboard;

public partial class StudySessionViewModel : ViewModelBase
{
    private readonly LearningProgressService _learningProgressService;
    private readonly INavigationService _navigationService;

    private DispatcherTimer _timer = null!;
    private TimeSpan _total;
    private TimeSpan _remaining;

    private Guid _unitId;

    private const double CircleLength = 603;

    [ObservableProperty] private string title = "";
    [ObservableProperty] private string remainingTimeText = "00:00";
    [ObservableProperty] private double progressDashOffset;
    [ObservableProperty] private bool isRunning;
    [ObservableProperty] private bool isPaused;

    public StudySessionViewModel(
        LearningProgressService learningProgressService,
        INavigationService navigationService)
    {
        _learningProgressService = learningProgressService;
        _navigationService = navigationService;
    }

    public void Init(LearningProgressEntity unit)
    {
        _unitId = unit.Id;

        Title = $"{unit.Subject} – {unit.ModuleName}";

        _total = unit.PlannedEnd.TimeOfDay - unit.PlannedStart.TimeOfDay;
        _remaining = _total;

        UpdateUi();

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += (_, _) => Tick();

        Start();
    }

    private void Tick()
    {
        if (_remaining <= TimeSpan.Zero)
        {
            _ = FinishAsync();
            return;
        }

        _remaining -= TimeSpan.FromSeconds(1);
        UpdateUi();
    }

    private void UpdateUi()
    {
        RemainingTimeText =
            $"{(int)_remaining.TotalMinutes:D2}:{_remaining.Seconds:D2}";

        var progress = _remaining.TotalSeconds / _total.TotalSeconds;
        ProgressDashOffset = CircleLength * (1 - progress);
    }

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
        IsRunning = false;
        IsPaused = true;
        _timer.Stop();
    }

    [RelayCommand]
    private async Task FinishAsync()
    {
        _timer.Stop();
        await _learningProgressService.MarkCompletedAsync(_unitId);
        _navigationService.NavigateTo<ShellViewModel>();
    }
}
