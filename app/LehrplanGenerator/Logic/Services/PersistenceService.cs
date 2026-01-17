using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LehrplanGenerator.Data.Repositories;
using LehrplanGenerator.Logic.Models;

namespace LehrplanGenerator.Logic.Services;

/// <summary>
/// Service zum Speichern von Chat-Messages und Kalendern in SQLite3
/// </summary>
public class PersistenceService
{
    private readonly ICalendarRepository _calendarRepository;
    private readonly IChatRepository _chatRepository;

    public PersistenceService(ICalendarRepository calendarRepository, IChatRepository chatRepository)
    {
        _calendarRepository = calendarRepository;
        _chatRepository = chatRepository;
    }

    /// <summary>
    /// Speichere eine Chat-Message in der Datenbank
    /// </summary>
    public async Task SaveChatMessageAsync(Guid userId, Guid sessionId, string sender, string text)
    {
        await _chatRepository.SaveMessageAsync(sessionId, userId, sender, text);
    }

    /// <summary>
    /// Speichere einen kompletten Kalender (DayPlan) mit Tasks
    /// </summary>
    public async Task SaveCalendarAsync(Guid userId, string day, DayPlan dayPlan)
    {
        // Überprüfe ob ein DayPlan für diesen Tag bereits existiert
        var existingDayPlan = await _calendarRepository.GetDayPlanAsync(userId, day);

        DayPlanEntity dayPlanEntity;

        if (existingDayPlan != null)
        {
            // Update existierenden DayPlan
            dayPlanEntity = existingDayPlan;
            dayPlanEntity.Tasks.Clear();
        }
        else
        {
            // Neuen DayPlan erstellen
            dayPlanEntity = new DayPlanEntity
            {
                UserId = userId,
                Day = day,
                CreatedAt = DateTime.UtcNow
            };
        }

        // Füge alle Tasks hinzu
        if (dayPlan.Tasks != null)
        {
            foreach (var task in dayPlan.Tasks)
            {
                var taskEntity = TaskItemEntity.FromTaskItem(task, dayPlanEntity.DayPlanId);
                dayPlanEntity.Tasks.Add(taskEntity);
            }
        }

        // Speichere
        if (existingDayPlan != null)
        {
            await _calendarRepository.UpdateDayPlanAsync(dayPlanEntity);
        }
        else
        {
            await _calendarRepository.AddDayPlanAsync(dayPlanEntity);
        }
    }

    /// <summary>
    /// Update den Checkbox-Status einer Task (IsDone)
    /// </summary>
    public async Task UpdateTaskStatusAsync(Guid taskId, bool isDone)
    {
        var task = await _calendarRepository.GetTaskAsync(taskId);
        if (task != null)
        {
            task.IsDone = isDone;
            await _calendarRepository.UpdateTaskAsync(task);
        }
    }

    /// <summary>
    /// Lade alle Kalender eines Nutzers
    /// </summary>
    public async Task<IEnumerable<DayPlanEntity>> LoadUserCalendarsAsync(Guid userId)
    {
        return await _calendarRepository.GetUserDayPlansAsync(userId);
    }
}
