using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LehrplanGenerator.Logic.Utils;

public sealed partial class ThemeManager : ObservableObject
{
    public static ThemeManager Instance { get; } = new();

    private ThemeManager()
    {
        ThemeVariant = ThemeVariant.Light;
    }

    [ObservableProperty]
    private ThemeVariant themeVariant;

    public bool IsDark
    {
        get => ThemeVariant == ThemeVariant.Dark;
        set => ThemeVariant = value ? ThemeVariant.Dark : ThemeVariant.Light;
    }

    public void Toggle()
    {
        IsDark = !IsDark;
    }
}
