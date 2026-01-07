using CommunityToolkit.Mvvm.ComponentModel;
using System;
using LehrplanGenerator.Logic.AI;
using LehrplanGenerator.Logic.Models;

namespace LehrplanGenerator.Logic.State;

public partial class AppState : ObservableObject
{
    [ObservableProperty]
    private Guid? _currentUserId;

    [ObservableProperty]
    private string? _currentUserDisplayName;

    [ObservableProperty]
    private StudyPlan? _currentStudyPlan;

    // Geteiltes AI-Service für Chat und StudyPlan
    public StudyPlanGeneratorService AiService { get; } = new StudyPlanGeneratorService();
}
