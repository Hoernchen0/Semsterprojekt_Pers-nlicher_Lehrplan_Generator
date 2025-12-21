using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace LehrplanGenerator.Logic.State;

public partial class AppState : ObservableObject
{
    [ObservableProperty]
    private Guid? _currentUserId;

    [ObservableProperty]
    private string? _currentUserDisplayName;
}
