using CommunityToolkit.Mvvm.ComponentModel;

namespace LehrplanGenerator.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Lehrplangenerator!";
}
