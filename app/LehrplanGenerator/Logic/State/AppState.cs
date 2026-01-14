using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using LehrplanGenerator.Logic.AI;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Models.Chat;

namespace LehrplanGenerator.Logic.State;

public partial class AppState : ObservableObject
{
    [ObservableProperty]
    private Guid? _currentUserId;

    [ObservableProperty]
    private string? _currentUserDisplayName;

    [ObservableProperty]
    private StudyPlan? _currentStudyPlan;

    // Chat-Nachrichten bleiben während der Session erhalten
    public ObservableCollection<ChatMessage> ChatMessages { get; } = new ObservableCollection<ChatMessage>();

    // Geteiltes AI-Service für Chat und StudyPlan
    public StudyPlanGeneratorService AiService { get; } = new StudyPlanGeneratorService();
}
