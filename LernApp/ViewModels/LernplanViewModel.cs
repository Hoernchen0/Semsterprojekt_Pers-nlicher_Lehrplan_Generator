using System.Collections.ObjectModel;
using LernApp.Models;
using LernApp.Services;

namespace LernApp.ViewModels;

public class LernplanViewModel
{
    private readonly DatabaseService _db = new DatabaseService();
    public ObservableCollection<LernEinheit> Einheiten { get; } = new ObservableCollection<LernEinheit>();

    public LernplanViewModel()
    {
        LadeDatenAsync();
    }

    public async void LadeDatenAsync()
    {
        Einheiten.Clear();
        var daten = await _db.LadeAlleAsync();
        foreach (var e in daten)
            Einheiten.Add(e);
    }

    public async void NeueEinheit(string fach, string thema)
    {
        var einheit = new LernEinheit
        {
            Fach = fach,
            Thema = thema,
            Datum = DateTime.Now
        };

        await _db.SpeichernAsync(einheit);
        LadeDatenAsync();
    }
}
