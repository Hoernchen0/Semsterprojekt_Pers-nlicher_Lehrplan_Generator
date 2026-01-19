using CommunityToolkit.Mvvm.ComponentModel;

namespace LehrplanGenerator.Models.Chat;

public partial class ChatMessage : ObservableObject
{
    public string Sender { get; set; } = "";

    // 👇 FÜR DB / KOMPATIBILITÄT
    public string Text
    {
        get => FullText;
        set => FullText = value;
    }

    public string FullText { get; set; } = "";

    public bool IsUser => Sender == "User";
    public bool IsAi => Sender == "AI";
    public bool IsSystem => Sender == "System";

    [ObservableProperty]
    private string displayedText = "";
}
