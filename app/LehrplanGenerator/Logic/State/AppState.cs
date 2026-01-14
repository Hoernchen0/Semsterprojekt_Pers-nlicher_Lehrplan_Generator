using CommunityToolkit.Mvvm.ComponentModel;
using System;
using LehrplanGenerator.Logic.AI;
using LehrplanGenerator.Logic.Models;

namespace LehrplanGenerator.Logic.State;

public partial class AppState : ObservableObject
{
    public static LehrplanGenerator.Logic.Models.User? CurrentUser { get; set; }

    public static Guid? CurrentUserId => CurrentUser?.UserId;

    public static string? CurrentUserDisplayName => CurrentUser?.DisplayName;

    [ObservableProperty]
    private StudyPlan? _currentStudyPlan;

    // Geteiltes AI-Service für Chat und StudyPlan
    public StudyPlanGeneratorService AiService { get; } = new StudyPlanGeneratorService();
}
