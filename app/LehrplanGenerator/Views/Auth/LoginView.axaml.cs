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

public partial class LoginView : UserControl
{
    private const double DesktopBreakpoint = 720;
    private const double AndroidKeyboardOffset = 300;

    private IInputPane? _inputPane;
    private readonly TranslateTransform _mobileSheetTransform = new();

    public LoginView()
    {
        InitializeComponent();

        // ================= LAYOUT =================
        AttachedToVisualTree += OnAttachedToVisualTree;
        DetachedFromVisualTree += OnDetachedFromVisualTree;
        SizeChanged += (_, _) => UpdateResponsiveLayout();

        // ================= DESKTOP =================
        DesktopUsernameBox.KeyDown += (_, e) =>
        {
            if (e.Key == Key.Enter)
            {
                DesktopPasswordBox.Focus();
                e.Handled = true;
            }
        };

        DesktopPasswordBox.KeyDown += (_, e) =>
        {
            if (e.Key == Key.Enter)
            {
                ExecuteLogin();
                e.Handled = true;
            }
        };

        // ================= MOBILE =================
        MobileUsernameBox.KeyDown += OnMobileUsernameKeyDown;
        MobilePasswordBox.KeyDown += OnMobilePasswordKeyDown;

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
            if (e.NewState == InputPaneState.Open)
                _mobileSheetTransform.Y = -AndroidKeyboardOffset;
            else
                _mobileSheetTransform.Y = 0;
        }, DispatcherPriority.Render);
    }

    // =========================================================
    // MOBILE HANDLING
    // =========================================================

    private void OnMobileUsernameKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            Dispatcher.UIThread.Post(() => MobilePasswordBox.Focus());
        }
    }

    private void OnMobilePasswordKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            ExecuteLogin();
        }
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
    // SHARED
    // =========================================================

    private void ExecuteLogin()
    {
        if (DataContext is LoginViewModel vm &&
            vm.LoginCommand.CanExecute(null))
        {
            vm.LoginCommand.Execute(null);
        }
    }

    private void UpdateResponsiveLayout()
    {
        var width = Bounds.Width;
        if (width <= 0)
            return;

        DesktopLayout.IsVisible = width >= DesktopBreakpoint;
        MobileLayout.IsVisible = width < DesktopBreakpoint;
    }
}
