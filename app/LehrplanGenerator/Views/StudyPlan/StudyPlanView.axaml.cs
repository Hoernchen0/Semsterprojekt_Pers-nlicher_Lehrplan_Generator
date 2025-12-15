using Avalonia.Controls;
using LehrplanGenerator.ViewModels;
using LehrplanGenerator.Views.Windows;
using LehrplanGenerator.Logic.Services;

namespace LehrplanGenerator.Views.StudyPlan;

public partial class StudyPlanView : UserControl
{

    private readonly MainWindow _main;

    public StudyPlanView(MainWindow mainWindow)
    {
        InitializeComponent();
        _main = mainWindow;
        DataContext = new StudyPlanViewModel(mainWindow);
    }
}