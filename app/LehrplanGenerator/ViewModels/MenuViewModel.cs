using CommunityToolkit.Mvvm.ComponentModel;
using LehrplanGenerator.Logic.State;

namespace LehrplanGenerator.ViewModels;

public partial class MenuViewModel : ObservableObject
{
    [ObservableProperty]
    private string welcomeMessage = "";

    public MenuViewModel()
    {
        if (AppState.CurrentUser != null)
        {
            WelcomeMessage = $"Willkommen, {AppState.CurrentUser.Name}";
        }
        else
        {
            WelcomeMessage = "Willkommen";
        }
    }
}
