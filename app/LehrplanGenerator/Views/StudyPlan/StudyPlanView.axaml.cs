using Avalonia.Controls;

namespace LehrplanGenerator.Views.StudyPlan;

public partial class StudyPlanView : UserControl
{
    private const double DesktopBreakpoint = 900;

    public StudyPlanView()
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
