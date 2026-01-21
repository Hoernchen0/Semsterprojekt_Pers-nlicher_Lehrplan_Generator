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

    public MainWindowViewModel(INavigationService navigationService)
    {
        navigationService.SetMainViewModel(this);
        navigationService.NavigateTo<MainViewModel>();
    }
}