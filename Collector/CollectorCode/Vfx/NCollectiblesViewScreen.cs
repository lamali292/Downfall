using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;
using MegaCrit.Sts2.Core.TestSupport;

namespace Collector.CollectorCode.Vfx;

[GlobalClass]
public partial class NCollectiblesViewScreen : NCardsViewScreen
{
    private readonly List<SortingOrders> _sortingPriority;
    private NCardViewSortButton? _alphabetSorter;
    private Control? _bg;
    private NCardViewSortButton? _costSorter;
    private NCardViewSortButton? _obtainedSorter;

    private Player? _player;
    private NCardViewSortButton? _typeSorter;

    public NCollectiblesViewScreen()
    {
        _sortingPriority =
        [
            SortingOrders.Ascending,
            SortingOrders.TypeAscending,
            SortingOrders.CostAscending,
            SortingOrders.AlphabetAscending
        ];
    }

    private static string ScenePath => "res://Collector/scenes/ui/collectible_view_screen.tscn";

    public override NetScreenType ScreenType => NetScreenType.DeckView;

    public static NCollectiblesViewScreen? ShowScreen(Player player, List<CardModel> cards)
    {
        if (TestMode.IsOn || NCapstoneContainer.Instance == null)
            return null;
        var screen = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NCollectiblesViewScreen>();
        screen._player = player;
        screen._cards = cards;
        NDebugAudioManager.Instance?.Play("map_open.mp3");
        Callable.From(() => NCapstoneContainer.Instance.Open(screen)).CallDeferred();
        return screen;
    }

    public override void _Ready()
    {
        if (_player == null) return;
        _infoText = new LocString("gameplay_ui", "DECK_PILE_INFO");
        _bg = GetNode<Control>("%SortingBg");
        _obtainedSorter = GetNode<NCardViewSortButton>("%ObtainedSorter");
        _typeSorter = GetNode<NCardViewSortButton>("%CardTypeSorter");
        _costSorter = GetNode<NCardViewSortButton>("%CostSorter");
        _alphabetSorter = GetNode<NCardViewSortButton>("%AlphabeticalSorter");
        _obtainedSorter.Connect(NClickableControl.SignalName.Released,
            Callable.From(new Action<NButton>(OnObtainedSort)));
        _typeSorter.Connect(NClickableControl.SignalName.Released, Callable.From(new Action<NButton>(OnCardTypeSort)));
        _costSorter.Connect(NClickableControl.SignalName.Released, Callable.From(new Action<NButton>(OnCostSort)));
        _alphabetSorter.Connect(NClickableControl.SignalName.Released,
            Callable.From(new Action<NButton>(OnAlphabetSort)));
        _obtainedSorter.SetLabel(new LocString("gameplay_ui", "SORT_OBTAINED").GetRawText());
        _typeSorter.SetLabel(new LocString("gameplay_ui", "SORT_TYPE").GetRawText());
        _costSorter.SetLabel(new LocString("gameplay_ui", "SORT_COST").GetRawText());
        _alphabetSorter.SetLabel(new LocString("gameplay_ui", "SORT_ALPHABET").GetRawText());
        GetNode<MegaLabel>("%ViewUpgradesLabel")
            .SetTextAutoSize(new LocString("gameplay_ui", "VIEW_UPGRADES").GetFormattedText());
        var frameMaterial = (ShaderMaterial)_player.Character.CardPool.FrameMaterial;
        _bg.Material = frameMaterial;
        _obtainedSorter.SetHue(frameMaterial);
        _typeSorter.SetHue(frameMaterial);
        _costSorter.SetHue(frameMaterial);
        _alphabetSorter.SetHue(frameMaterial);
        ConnectSignals();
        DisplayCards();
        var controlArray = new Control[] { _obtainedSorter, _typeSorter, _costSorter, _alphabetSorter };
        for (var index = 0; index < controlArray.Length; ++index)
        {
            controlArray[index].FocusNeighborTop = controlArray[index].GetPath();
            controlArray[index].FocusNeighborBottom =
                _grid.DefaultFocusedControl?.GetPath() ?? controlArray[index].GetPath();
            controlArray[index].FocusNeighborLeft =
                index > 0 ? controlArray[index - 1].GetPath() : controlArray[index].GetPath();
            controlArray[index].FocusNeighborRight = index < controlArray.Length - 1
                ? controlArray[index + 1].GetPath()
                : controlArray[index].GetPath();
        }
    }

    public override void AfterCapstoneClosed()
    {
        base.AfterCapstoneClosed();
        NTopBarCollectorButton.RefreshButton();
    }

    private void OnObtainedSort(NButton button)
    {
        if (_obtainedSorter == null) return;
        _sortingPriority.Remove(SortingOrders.Ascending);
        _sortingPriority.Remove(SortingOrders.Descending);
        _sortingPriority.Insert(0, _obtainedSorter.IsDescending ? SortingOrders.Descending : SortingOrders.Ascending);
        DisplayCards();
    }

    private void OnCardTypeSort(NButton button)
    {
        if (_typeSorter == null) return;
        _sortingPriority.Remove(SortingOrders.TypeAscending);
        _sortingPriority.Remove(SortingOrders.TypeDescending);
        _sortingPriority.Insert(0,
            _typeSorter.IsDescending ? SortingOrders.TypeDescending : SortingOrders.TypeAscending);
        DisplayCards();
    }

    private void OnCostSort(NButton button)
    {
        if (_costSorter == null) return;
        _sortingPriority.Remove(SortingOrders.CostAscending);
        _sortingPriority.Remove(SortingOrders.CostDescending);
        _sortingPriority.Insert(0,
            _costSorter.IsDescending ? SortingOrders.CostDescending : SortingOrders.CostAscending);
        DisplayCards();
    }

    private void OnAlphabetSort(NButton button)
    {
        if (_alphabetSorter == null) return;
        _sortingPriority.Remove(SortingOrders.AlphabetAscending);
        _sortingPriority.Remove(SortingOrders.AlphabetDescending);
        _sortingPriority.Insert(0,
            _alphabetSorter.IsDescending ? SortingOrders.AlphabetDescending : SortingOrders.AlphabetAscending);
        DisplayCards();
    }

    private void DisplayCards()
    {
        if (_obtainedSorter == null) return;
        _grid.YOffset = 100;
        _grid.SetCards(_cards, PileType.None, _sortingPriority);
        var topRowOfCardNodes = _grid.GetTopRowOfCardNodes();
        if (topRowOfCardNodes == null) return;
        foreach (var control in topRowOfCardNodes)
            control.FocusNeighborTop = _obtainedSorter.GetPath();
    }
}