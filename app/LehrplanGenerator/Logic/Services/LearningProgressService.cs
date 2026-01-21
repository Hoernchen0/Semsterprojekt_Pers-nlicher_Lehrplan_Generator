using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using LehrplanGenerator.Data;
using LehrplanGenerator.Logic.Models;
using Microsoft.EntityFrameworkCore;

namespace LehrplanGenerator.Logic.Services;

public class LearningProgressService
{
    private readonly LehrplanDbContext _db;

    public LearningProgressService(LehrplanDbContext db)
    {
        _db = db;
    }

    // =========================
    // SAVE STUDY PLAN (ATOMIC)
    // =========================
    public async Task SaveStudyPlanAsync(Guid userId, StudyPlan plan)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            var studyPlanEntity = new StudyPlanEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = plan.Topic,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.StudyPlans.Add(studyPlanEntity);

            foreach (var day in plan.Days)
            {
                var date = DateTime.ParseExact(
                    day.Day,
                    "dd.MM.yyyy",
                    CultureInfo.InvariantCulture
                );

                foreach (var task in day.Tasks)
                {
                    var plannedStart = DateTime.ParseExact(
                        $"{date:yyyy-MM-dd} {task.StartTime}",
                        "yyyy-MM-dd HH:mm",
                        CultureInfo.InvariantCulture
                    );

                    var plannedEnd = DateTime.ParseExact(
                        $"{date:yyyy-MM-dd} {task.EndTime}",
                        "yyyy-MM-dd HH:mm",
                        CultureInfo.InvariantCulture
                    );

                    if (plannedEnd <= plannedStart)
                        plannedEnd = plannedStart.AddMinutes(45);

                    studyPlanEntity.LearningUnits.Add(new LearningProgressEntity
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        StudyPlanId = studyPlanEntity.Id,
                        Subject = plan.Topic,
                        ModuleName = task.Title,
                        Chapter = task.Description,
                        PlannedStart = plannedStart.ToUniversalTime(),
                        PlannedEnd = plannedEnd.ToUniversalTime(),
                        IsCompleted = task.IsDone,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            await _db.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine(ex.InnerException?.Message);
            throw;
        }
    }

    // =========================
    // LOAD ALL STUDY PLANS
    // =========================
    public async Task<List<StudyPlanEntity>> LoadStudyPlansAsync(Guid userId)
    {
        return await _db.StudyPlans
            .Where(sp => sp.UserId == userId)
            .OrderByDescending(sp => sp.CreatedAt)
            .ToListAsync();
    }

    // =========================
    // LOAD SINGLE STUDY PLAN
    // =========================
    public async Task<(string Title, Dictionary<string, List<LearningProgressEntity>> GroupedDays)>
        LoadStudyPlanAsync(Guid studyPlanId)
    {
        var units = await _db.LearningProgress
            .Where(lp => lp.StudyPlanId == studyPlanId)
            .OrderBy(lp => lp.PlannedStart)
            .ToListAsync();

        if (units.Count == 0)
            return (string.Empty, new Dictionary<string, List<LearningProgressEntity>>());

        var grouped = units
            .GroupBy(u => u.PlannedStart.Date)
            .ToDictionary(
                g => g.Key.ToString("dd.MM.yyyy"),
                g => g.ToList()
            );

        return (units.First().Subject, grouped);
    }

    // =========================
    // MARK COMPLETED
    // =========================
    public async Task MarkCompletedAsync(Guid id, bool done = true)
    {
        var unit = await _db.LearningProgress.FirstOrDefaultAsync(x => x.Id == id);
        if (unit == null)
            return;

        unit.IsCompleted = done;
        unit.CompletedAt = done ? DateTime.UtcNow : null;
        unit.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
    }

    // =========================
    // DASHBOARD HELPERS
    // =========================
    public async Task<int> GetProgressPercentAsync(Guid studyPlanId)
    {
        var total = await _db.LearningProgress.CountAsync(lp => lp.StudyPlanId == studyPlanId);
        if (total == 0)
            return 0;

        var done = await _db.LearningProgress.CountAsync(
            lp => lp.StudyPlanId == studyPlanId && lp.IsCompleted
        );

        return (int)Math.Round((double)done / total * 100);
    }

    public async Task<LearningProgressEntity?> GetNextUnitAsync(Guid studyPlanId)
    {
        return await _db.LearningProgress
            .Where(lp => lp.StudyPlanId == studyPlanId && !lp.IsCompleted)
            .OrderBy(lp => lp.PlannedStart)
            .FirstOrDefaultAsync();
    }
}
