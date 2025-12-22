using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace LehrplanGenerator.Logic.State;

public partial class AppState : ObservableObject
{
    public static LehrplanGenerator.Logic.Models.User? CurrentUser { get; set; }
}
