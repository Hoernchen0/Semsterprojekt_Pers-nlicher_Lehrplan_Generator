using System;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Input;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Threading;
using LehrplanGenerator.ViewModels.Auth;

namespace LehrplanGenerator.Views.Auth;

public partial class RegisterView : UserControl
{
    private const double DesktopBreakpoint = 720;
    private const double AndroidKeyboardOffset = 300;

    private IInputPane? _inputPane;
    private readonly TranslateTransform _mobileSheetTransform = new();

    public RegisterView()
    {
        InitializeComponent();

        // ================= LAYOUT =================
        AttachedToVisualTree += OnAttachedToVisualTree;
        DetachedFromVisualTree += OnDetachedFromVisualTree;
        SizeChanged += (_, _) => UpdateResponsiveLayout();

        SetupMobileSheet();
    }

    // =========================================================
    // VISUAL TREE / INPUT PANE
    // =========================================================

    private void OnAttachedToVisualTree(object? sender, EventArgs e)
    {
        UpdateResponsiveLayout();

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null)
            return;

        _inputPane = topLevel.InputPane;
        if (_inputPane != null)
            _inputPane.StateChanged += OnInputPaneStateChanged;
    }

    private void OnDetachedFromVisualTree(object? sender, EventArgs e)
    {
        if (_inputPane != null)
            _inputPane.StateChanged -= OnInputPaneStateChanged;
    }

    // =========================================================
    // INPUT PANE (ANDROID KEYBOARD)
    // =========================================================

    private void OnInputPaneStateChanged(object? sender, InputPaneStateEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            _mobileSheetTransform.Y =
                e.NewState == InputPaneState.Open
                    ? -AndroidKeyboardOffset
                    : 0;
        }, DispatcherPriority.Render);
    }

    // =========================================================
    // SETUP
    // =========================================================

    private void SetupMobileSheet()
    {
        MobileSheet.RenderTransform = _mobileSheetTransform;

        MobileSheet.Transitions = new Transitions
        {
            new DoubleTransition
            {
                Property = TranslateTransform.YProperty,
                Duration = TimeSpan.FromMilliseconds(220),
                Easing = new CubicEaseOut()
            }
        };
    }

    // =========================================================
    // RESPONSIVE
    // =========================================================

    private void UpdateResponsiveLayout()
    {
        var width = Bounds.Width;
        if (width <= 0)
            return;

        DesktopLayout.IsVisible = width >= DesktopBreakpoint;
        MobileLayout.IsVisible = width < DesktopBreakpoint;
    }
}
