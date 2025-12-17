using System;
using Microsoft.Extensions.DependencyInjection;
using LehrplanGenerator.ViewModels;
using LehrplanGenerator.ViewModels.Windows;

namespace LehrplanGenerator.Logic.Services;

public interface INavigationService
{
    void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
    void SetMainViewModel(MainWindowViewModel mainViewModel);
}

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private MainWindowViewModel? _mainViewModel;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void SetMainViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {
        if (_mainViewModel == null)
            throw new InvalidOperationException("MainViewModel not set. Call SetMainViewModel first.");

        var viewModel = _serviceProvider.GetRequiredService<TViewModel>();

        _mainViewModel.CurrentViewModel = viewModel;
    }
}