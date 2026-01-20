// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
// using LehrplanGenerator.Logic.Models;

// namespace LehrplanGenerator.Data.Repositories;

// public interface ICalendarRepository
// {
//     Task<DayPlanEntity?> GetDayPlanAsync(Guid userId, string day);
//     Task<IEnumerable<DayPlanEntity>> GetUserDayPlansAsync(Guid userId);
//     Task AddDayPlanAsync(DayPlanEntity dayPlan);
//     Task UpdateDayPlanAsync(DayPlanEntity dayPlan);
//     Task DeleteDayPlanAsync(Guid dayPlanId);
//     Task<TaskItemEntity?> GetTaskAsync(Guid taskId);
//     Task UpdateTaskAsync(TaskItemEntity task);
//     Task AddTaskAsync(TaskItemEntity task);
//     Task DeleteTaskAsync(Guid taskId);
// }

// public class CalendarRepository : ICalendarRepository
// {
//     private readonly LehrplanDbContext _context;

//     public CalendarRepository(LehrplanDbContext context)
//     {
//         _context = context;
//     }

//     /// <summary>
//     /// Hole einen DayPlan für einen bestimmten Tag eines Nutzers
//     /// </summary>
//     public async Task<DayPlanEntity?> GetDayPlanAsync(Guid userId, string day)
//     {
//         return await _context.DayPlans
//             .Include(d => d.Tasks)
//             .FirstOrDefaultAsync(d => d.UserId == userId && d.Day == day);
//     }

//     /// <summary>
//     /// Hole alle DayPlans eines Nutzers
//     /// </summary>
//     public async Task<IEnumerable<DayPlanEntity>> GetUserDayPlansAsync(Guid userId)
//     {
//         return await _context.DayPlans
//             .Where(d => d.UserId == userId)
//             .Include(d => d.Tasks)
//             .OrderByDescending(d => d.CreatedAt)
//             .ToListAsync();
//     }

//     /// <summary>
//     /// Erstelle einen neuen DayPlan (Kalender-Eintrag)
//     /// </summary>
//     public async Task AddDayPlanAsync(DayPlanEntity dayPlan)
//     {
//         _context.DayPlans.Add(dayPlan);
//         await _context.SaveChangesAsync();
//     }

//     /// <summary>
//     /// Update einen existierenden DayPlan
//     /// </summary>
//     public async Task UpdateDayPlanAsync(DayPlanEntity dayPlan)
//     {
//         dayPlan.UpdatedAt = DateTime.UtcNow;
//         _context.DayPlans.Update(dayPlan);
//         await _context.SaveChangesAsync();
//     }

//     /// <summary>
//     /// Lösche einen DayPlan und alle seine Tasks
//     /// </summary>
//     public async Task DeleteDayPlanAsync(Guid dayPlanId)
//     {
//         var dayPlan = await _context.DayPlans.FindAsync(dayPlanId);
//         if (dayPlan != null)
//         {
//             _context.DayPlans.Remove(dayPlan);
//             await _context.SaveChangesAsync();
//         }
//     }

//     /// <summary>
//     /// Hole eine einzelne Task
//     /// </summary>
//     public async Task<TaskItemEntity?> GetTaskAsync(Guid taskId)
//     {
//         return await _context.TaskItems.FindAsync(taskId);
//     }

//     /// <summary>
//     /// Update eine Task (z.B. IsDone Status wenn Checkbox geändert wird)
//     /// </summary>
//     public async Task UpdateTaskAsync(TaskItemEntity task)
//     {
//         task.UpdatedAt = DateTime.UtcNow;
//         _context.TaskItems.Update(task);
//         await _context.SaveChangesAsync();
//     }

//     /// <summary>
//     /// Erstelle eine neue Task
//     /// </summary>
//     public async Task AddTaskAsync(TaskItemEntity task)
//     {
//         _context.TaskItems.Add(task);
//         await _context.SaveChangesAsync();
//     }

//     /// <summary>
//     /// Lösche eine Task
//     /// </summary>
//     public async Task DeleteTaskAsync(Guid taskId)
//     {
//         var task = await _context.TaskItems.FindAsync(taskId);
//         if (task != null)
//         {
//             _context.TaskItems.Remove(task);
//             await _context.SaveChangesAsync();
//         }
//     }
// }
