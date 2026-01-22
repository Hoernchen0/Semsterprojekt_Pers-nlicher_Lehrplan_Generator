using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace LehrplanGenerator.ViewModels.StudyPlan;

public class CalendarViewModel : ViewModelBase
{
    public ObservableCollection<DayPlanViewModel> Days { get; } = new();

    private readonly StudyPlanViewModel _studyPlanViewModel;
    private DateTime _currentMonth = DateTime.Today;
    public DateTime CurrentMonth
    {
        get => _currentMonth;
        set
        {
            _currentMonth = value;
            OnPropertyChanged(nameof(CurrentMonth));
            GenerateMonth();
        }
    }

    public ICommand NextMonthCommand { get; }
    public ICommand PreviousMonthCommand { get; }
    public ICommand SelectDayCommand { get; }

    public DayPlanViewModel? SelectedDay { get; set; }

    public CalendarViewModel(Action<DayPlanViewModel> openPopup, StudyPlanViewModel studyPlanViewModel)
    {
        _openPopup = openPopup;
        _studyPlanViewModel = studyPlanViewModel;

        NextMonthCommand = new RelayCommand(() =>
        {
            CurrentMonth = CurrentMonth.AddMonths(1);
        });

        PreviousMonthCommand = new RelayCommand(() =>
        {
            CurrentMonth = CurrentMonth.AddMonths(-1);
        });

        SelectDayCommand = new RelayCommand<DayPlanViewModel>(day =>
        {
            _openPopup(day);
        });

        GenerateMonth();
    }

    private readonly Action<DayPlanViewModel> _openPopup;


    public void GenerateMonth()
    {
        Days.Clear();

        var firstDayOfMonth = new DateTime(CurrentMonth.Year, CurrentMonth.Month, 1);
        var daysInMonth = DateTime.DaysInMonth(CurrentMonth.Year, CurrentMonth.Month);

        // Offset für den ersten Wochentag (Montag = 0, Sonntag = 6)
        int startOffset = ((int)firstDayOfMonth.DayOfWeek + 6) % 7;

        // Vormonats-Tage
        for (int i = 0; i < startOffset; i++)
        {
            var date = firstDayOfMonth.AddDays(-startOffset + i);
            AddDay(date, false);
        }

        // Tage des aktuellen Monats
        for (int i = 0; i < daysInMonth; i++)
        {
            var date = firstDayOfMonth.AddDays(i);
            AddDay(date, true);
        }

        // Folgemonats-Tage, um die letzte Woche zu füllen
        int totalDays = startOffset + daysInMonth;
        int remaining = 7 - (totalDays % 7);
        if (remaining < 7)
        {
            for (int i = 0; i < remaining; i++)
            {
                var date = firstDayOfMonth.AddDays(daysInMonth + i);
                AddDay(date, false);
            }
        }
    }

    private void AddDay(DateTime date, bool isCurrentMonth)
    {
        var existingDay = _studyPlanViewModel.Days.FirstOrDefault(d => d.ParsedDate.Date == date.Date);
        var tasks = existingDay != null
            ? new ObservableCollection<TaskItemViewModel>(existingDay.Tasks)
            : new ObservableCollection<TaskItemViewModel>();

        Days.Add(new DayPlanViewModel(date.ToString("dd.MM.yyyy"), tasks)
        {
            IsCurrentMonth = isCurrentMonth
        });
    }

}
