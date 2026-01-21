using System;
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

        if (!IsAndroid)
            return;

        MobileLayout.RenderTransform = _mobileTransform;

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
        if (_vm?.HasMessages != true)
            return;

        Dispatcher.UIThread.Post(() =>
        {
            _mobileTransform.Y =
                e.NewState == InputPaneState.Open
                    ? -AndroidKeyboardOffset
                    : 0;

            MobileScroll?.ScrollToEnd();
        }, DispatcherPriority.Render);
    }

    private void OnMessagesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => ScrollToBottom();

    private void OnMessagePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ChatMessage.DisplayedText))
            ScrollToBottom();
    }

    private void ScrollToBottom()
    {
        Dispatcher.UIThread.Post(() =>
        {
            MobileScroll?.ScrollToEnd();
            ChatScrollViewer?.ScrollToEnd();
        });
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
