using CommunityToolkit.Mvvm.ComponentModel;
using LehrplanGenerator.Logic.State;
using LehrplanGenerator.Data.Repositories;
using LehrplanGenerator.Logic.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LehrplanGenerator.ViewModels.Dashboard;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly AppState _appState;
    private readonly ICalendarRepository? _calendarRepository;

    [ObservableProperty]
    private string welcomeMessage = string.Empty;

    [ObservableProperty]
    private string userName = string.Empty;

    public DashboardViewModel(AppState appState, ICalendarRepository? calendarRepository = null)
    {
        _appState = appState;
        _calendarRepository = calendarRepository;

        UserName = _appState.CurrentUserDisplayName ?? "Benutzer";
        WelcomeMessage = $"Willkommen zurück, {UserName}!";

        System.Diagnostics.Debug.WriteLine($"CurrentUserDisplayName: '{_appState.CurrentUserDisplayName}'");

        _appState.PropertyChanged += (_, __) =>
        {
            UserName = _appState.CurrentUserDisplayName ?? "Benutzer";
            WelcomeMessage = $"Willkommen zurück, {UserName}!";
        };
        
        // Lade alte Lernpläne beim Initialisieren
        LoadPreviousStudyPlansAsync();
    }
    
    private async void LoadPreviousStudyPlansAsync()
    {
        if (!_appState.CurrentUserId.HasValue || _calendarRepository == null)
        {
            if (_calendarRepository == null)
                Console.WriteLine("ℹ CalendarRepository nicht verfügbar");
            return;
        }
            
        try
        {
            Console.WriteLine("📂 Lade alte Lernpläne vom Benutzer...");
            
            // Hole alle DayPlans des Benutzers
            var dayPlans = await _calendarRepository.GetUserDayPlansAsync(_appState.CurrentUserId.Value);
            var dayPlanList = dayPlans.ToList();
            
            if (dayPlanList.Count == 0)
            {
                Console.WriteLine("ℹ Keine alten Lernpläne gefunden");
                return;
            }
            
            Console.WriteLine($"✓ {dayPlanList.Count} Lernplan-Tag(e) gefunden");
            
            // Konvertiere alle DayPlans zu einem StudyPlan
            var studyPlan = new LehrplanGenerator.Logic.Models.StudyPlan();
            
            foreach (var dayPlanEntity in dayPlanList.OrderBy(d => d.Day))
            {
                var dayPlan = dayPlanEntity.ToDayPlan();
                studyPlan.Days.Add(dayPlan);
                Console.WriteLine($"  • {dayPlan.Day}: {dayPlan.Tasks.Count} Tasks");
            }
            
            // Setze den Lernplan im AppState so dass StudyPlanViewModel ihn laden kann
            _appState.CurrentStudyPlan = studyPlan;
            
            Console.WriteLine($"✓ Lernplan mit {studyPlan.Days.Count} Tagen wiederhergestellt");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Fehler beim Laden der Lernpläne: {ex}");
        }
    }
}