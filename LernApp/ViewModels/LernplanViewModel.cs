using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using LernApp.Models;
using LernApp.Services;

namespace LernApp.ViewModels;

public class LernplanViewModel : ViewModelBase
{
    private readonly ILernplanService _lernplanService;
    private readonly IUserService _userService;
    private readonly IAIService _aiService;
    private readonly ILernAppLogger _logger;

    private int _currentUserId;
    private string _selectedFach = "";
    private string _neuesThema = "";
    private string _neueBeschreibung = "";
    private bool _isLoading;

    public ObservableCollection<LernEinheit> LernEinheiten { get; } = new();

    public ReactiveCommand<Unit, Unit> ErzeugeNeueLernEinheitCommand { get; }
    public ReactiveCommand<Unit, Unit> LadeLernEinheitenCommand { get; }
    public ReactiveCommand<LernEinheit, Unit> LöscheLernEinheitCommand { get; }

    public LernplanViewModel(
        ILernplanService lernplanService,
        IUserService userService,
        IAIService aiService,
        ILernAppLogger logger)
    {
        _lernplanService = lernplanService;
        _userService = userService;
        _aiService = aiService;
        _logger = logger;

        // Befehle definieren
        ErzeugeNeueLernEinheitCommand = ReactiveCommand.CreateFromTask(ErzeugeNeueLernEinheit);
        LadeLernEinheitenCommand = ReactiveCommand.CreateFromTask(LadeLernEinheiten);
        LöscheLernEinheitCommand = ReactiveCommand.CreateFromTask<LernEinheit>(LöscheLernEinheit);
    }

    public int CurrentUserId
    {
        get => _currentUserId;
        set => this.RaiseAndSetIfChanged(ref _currentUserId, value);
    }

    public string SelectedFach
    {
        get => _selectedFach;
        set => this.RaiseAndSetIfChanged(ref _selectedFach, value);
    }

    public string NeuesThema
    {
        get => _neuesThema;
        set => this.RaiseAndSetIfChanged(ref _neuesThema, value);
    }

    public string NeueBeschreibung
    {
        get => _neueBeschreibung;
        set => this.RaiseAndSetIfChanged(ref _neueBeschreibung, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    private async Task ErzeugeNeueLernEinheit()
    {
        try
        {
            IsLoading = true;

            if (string.IsNullOrWhiteSpace(SelectedFach) || string.IsNullOrWhiteSpace(NeuesThema))
            {
                _logger.LogWarning("Fach und Thema sind erforderlich");
                return;
            }

            var neueEinheit = await _lernplanService.ErstelleLernEinheitAsync(
                CurrentUserId,
                SelectedFach,
                NeuesThema,
                NeueBeschreibung);

            LernEinheiten.Add(neueEinheit);
            _logger.LogInfo($"Lerneinheit erstellt: {neueEinheit.Thema}");

            // Felder zurücksetzen
            NeuesThema = "";
            NeueBeschreibung = "";
        }
        catch (Exception ex)
        {
            _logger.LogError($"Fehler beim Erstellen: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LadeLernEinheiten()
    {
        try
        {
            IsLoading = true;
            LernEinheiten.Clear();

            var einheiten = await _lernplanService.HoleLernEinheitenAsync(CurrentUserId);
            foreach (var einheit in einheiten)
            {
                LernEinheiten.Add(einheit);
            }

            _logger.LogInfo($"{einheiten.Count()} Lerneinheiten geladen");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Fehler beim Laden: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LöscheLernEinheit(LernEinheit einheit)
    {
        try
        {
            await _lernplanService.LöscheLernEinheitAsync(einheit.Id);
            LernEinheiten.Remove(einheit);
            _logger.LogInfo("Lerneinheit gelöscht");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Fehler beim Löschen: {ex.Message}");
        }
    }

    public async Task InitialisiereDatenAsync(int userId)
    {
        CurrentUserId = userId;
        await LadeLernEinheiten();
    }
}