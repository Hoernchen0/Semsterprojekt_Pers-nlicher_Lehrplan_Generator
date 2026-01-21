using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using LehrplanGenerator.Logic.AI;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Models.Chat;

namespace LehrplanGenerator.Logic.State;

public partial class AppState : ObservableObject
{
    // =========================
    // AUTH / USER
    // =========================
    [ObservableProperty]
    private Guid? currentUserId;

    [ObservableProperty]
    private string? currentUserDisplayName;

    // =========================
    // STUDY PLAN STATE
    // =========================
    [ObservableProperty]
    private Guid? currentStudyPlanId;

    [ObservableProperty]
    private string? currentUsername;

    [ObservableProperty]
    private Guid? currentSessionId;
    public ObservableCollection<StudyPlanEntity> StudyPlans { get; }
        = new ObservableCollection<StudyPlanEntity>();

    // =========================
    // CHAT STATE
    // =========================
    public ObservableCollection<ChatMessage> ChatMessages { get; }
        = new ObservableCollection<ChatMessage>();

    // =========================
    // SERVICES
    // =========================
    public StudyPlanGeneratorService AiService { get; }
        = new StudyPlanGeneratorService();

    // =========================
    // DASHBOARD / SESSION
    // =========================
    public LearningProgressEntity? CurrentStudySession { get; set; }
}
