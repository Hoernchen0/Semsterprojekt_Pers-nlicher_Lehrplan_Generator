using Avalonia.Controls;

namespace LehrplanGenerator.Views.Shell;

public partial class ShellView : UserControl
{
    private const double DesktopBreakpoint = 720;

    public ShellView()
    {
        InitializeComponent();

        AttachedToVisualTree += (_, _) => UpdateResponsiveLayout();
        SizeChanged += (_, _) => UpdateResponsiveLayout();
    }

    private void UpdateResponsiveLayout()
    {
        var width = Bounds.Width;
        if (width <= 0) return;

        DesktopLayout.IsVisible = width >= DesktopBreakpoint;
        MobileLayout.IsVisible = width < DesktopBreakpoint;
    }
}
