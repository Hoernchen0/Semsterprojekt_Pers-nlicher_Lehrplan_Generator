using Avalonia.Controls;
using LehrplanGenerator.ViewModels.Chat;
using System.Collections.Specialized;

namespace LehrplanGenerator.Views.Chat;

public partial class ChatView : UserControl
{
    public ChatView()
    {
        InitializeComponent();

        AttachedToVisualTree += (_, _) =>
        {
            if (DataContext is ChatViewModel vm)
            {
                vm.Messages.CollectionChanged += OnMessagesChanged;
            }
        };

        DetachedFromVisualTree += (_, _) =>
        {
            if (DataContext is ChatViewModel vm)
            {
                vm.Messages.CollectionChanged -= OnMessagesChanged;
            }
        };
    }

    private void OnMessagesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ChatScrollViewer?.ScrollToEnd();
    }
}
