using Avalonia.Controls;

namespace LehrplanGenerator.Views.Guide;

public partial class GuideView : UserControl
{
    private const double DesktopBreakpoint = 720;

    public GuideView()
    {
        InitializeComponent();
        AttachedToVisualTree += (_, _) => UpdateLayout();
        SizeChanged += (_, _) => UpdateLayout();
    }

    private void UpdateLayout()
    {
        var width = Bounds.Width;
        if (width <= 0)
            return;

        DesktopLayout.IsVisible = width >= DesktopBreakpoint;
        MobileLayout.IsVisible = width < DesktopBreakpoint;
    }
}
