using LehrplanGenerator.Logic.Services;
using LehrplanGenerator.ViewModels.Main;

namespace LehrplanGenerator.ViewModels.Windows;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _currentViewModel = null!;

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public MainWindowViewModel(
        INavigationService navigationService,
        MainViewModel mainViewModel)  // ← MainViewModel direkt injecten
    {
        navigationService.SetMainViewModel(this);
        _currentViewModel = mainViewModel;  // ← Direkt setzen statt navigieren
    }
}