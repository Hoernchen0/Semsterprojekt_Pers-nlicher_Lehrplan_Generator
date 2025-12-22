using LehrplanGenerator.Logic.Models;

namespace LehrplanGenerator.Logic.State;

public static class AppState
{
    public static LehrplanGenerator.Logic.Models.User? CurrentUser { get; set; }
}