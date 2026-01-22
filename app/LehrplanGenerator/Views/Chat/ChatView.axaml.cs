using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Media;
using Avalonia.Threading;
using LehrplanGenerator.ViewModels.Chat;
using LehrplanGenerator.Models.Chat;
using System.Collections.Specialized;
using System.ComponentModel;

namespace LehrplanGenerator.Views.Chat;

public partial class ChatView : UserControl
{
    private const double DesktopBreakpoint = 720;
    private const double AndroidKeyboardOffset = 300;


    private readonly TranslateTransform _inputTransform = new();
    private ChatViewModel? _vm;
    private IInputPane? _inputPane;
    private readonly TranslateTransform _mobileTransform = new();

    private static bool IsAndroid => OperatingSystem.IsAndroid();

    public ChatView()
    {
        InitializeComponent();
        AttachedToVisualTree += OnAttached;
        DetachedFromVisualTree += OnDetached;
        SizeChanged += (_, _) => UpdateResponsiveLayout();


    }

    private void OnAttached(object? sender, EventArgs e)
    {
        _vm = DataContext as ChatViewModel;
        if (_vm == null)
            return;

        _vm.Messages.CollectionChanged += OnMessagesChanged;

        foreach (var m in _vm.Messages)
            m.PropertyChanged += OnMessagePropertyChanged;

        Dispatcher.UIThread.Post(ScrollToBottom, DispatcherPriority.Render);

        if (!IsAndroid)
            return;

        MobileScroll.RenderTransform = _mobileTransform;
        MobileInput.RenderTransform = _inputTransform;

        var top = TopLevel.GetTopLevel(this);
        _inputPane = top?.InputPane;
        if (_inputPane != null)
            _inputPane.StateChanged += OnInputPaneStateChanged;
    }

    private void OnDetached(object? sender, EventArgs e)
    {
        if (_vm != null)
        {
            _vm.Messages.CollectionChanged -= OnMessagesChanged;
            foreach (var m in _vm.Messages)
                m.PropertyChanged -= OnMessagePropertyChanged;
        }

        if (_inputPane != null)
            _inputPane.StateChanged -= OnInputPaneStateChanged;
    }

    private void OnInputPaneStateChanged(object? sender, InputPaneStateEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (e.NewState == InputPaneState.Open)
            {
                _mobileTransform.Y = -AndroidKeyboardOffset;
                _inputTransform.Y = -AndroidKeyboardOffset;
            }
            else
            {
                _mobileTransform.Y = 0;
                _inputTransform.Y = 0;
            }

            ScrollToBottom();
        }, DispatcherPriority.Render);
    }

    private void OnMessagesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
            ScrollToBottom();
    }

    private void OnMessagePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ChatMessage.DisplayedText))
            ScrollToBottom();
    }

    private void ScrollToBottom()
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (MobileScroll == null)
                return;

            if (MobileScroll.Extent.Height <= MobileScroll.Viewport.Height)
                return;

            MobileScroll.Offset = new Vector(
                0,
                MobileScroll.Extent.Height - MobileScroll.Viewport.Height
            );
        }, DispatcherPriority.Render);
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
