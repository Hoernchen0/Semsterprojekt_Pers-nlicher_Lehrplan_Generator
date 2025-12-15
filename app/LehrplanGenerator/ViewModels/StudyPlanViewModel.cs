using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LehrplanGenerator.Logic.Models;
using LehrplanGenerator.Logic.Services;
using System.Collections.ObjectModel;
using LehrplanGenerator.Views.Windows;


namespace LehrplanGenerator.ViewModels;

public partial class StudyPlanViewModel : ObservableObject
{
    [ObservableProperty] private string title = string.Empty;
    [ObservableProperty] private ObservableCollection<StudyEntry> _entries = new();
    [ObservableProperty] private string result = string.Empty;

    private readonly StudyPlanService _service;
    private readonly MainWindow _main;

    public StudyPlanViewModel(MainWindow mainWindow)
    {
        _main = mainWindow;
        _service = new StudyPlanService();
        Load("Assets/lernplan.xml");
    }

    [RelayCommand]
    public void Load(string path)
    {
        var plan = _service.LoadFromXml(path);

        Title = plan.Title;
        Entries.Clear();

        foreach (var entry in plan.Entries)
        {
            Entries.Add(entry);
        }

        Result = $"Plan '{Title}' erfolgreich geladen.";
    }

}
