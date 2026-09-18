using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;

namespace Downfall.DownfallCode.Voting;

/// <summary>
/// Card picker for the Upload Art flow. A functional/visual clone of
/// <see cref="MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary.NCardLibrary"/> -
/// same sidebar layout, same search/type/rarity/cost filter widgets, same
/// multi-key sort priority list - built against the base <see cref="NCardGrid"/>
/// rather than the <see cref="NCardLibraryGrid"/> subclass, which gates card
/// visibility on save/unlock state. Any card missing art must stay pickable
/// regardless of the moderator's own unlock progress.
///
/// Left out relative to the real screen: the pool row is built dynamically
/// from whichever Downfall characters actually have missing art (not a fixed
/// vanilla 8), and the MultiplayerCards/Stats/Upgrades tickboxes are skipped -
/// none of those systems (win-rate stats, upgrade preview, multiplayer-only
/// cards) exist for this picker's domain.
/// </summary>
public partial class NSelectCardPopup : Control
{
    public const string ScenePath = "res://Downfall/scenes/voting/select_card_popup.tscn";

    private const string CardGridScenePath = "res://scenes/cards/card_grid.tscn";
    private const string PoolFilterScenePath = "res://scenes/screens/card_library/library_pool_toggle.tscn";

    private static NSelectCardPopup? _current;

    public static void OpenFrom(Node parent, Action<ArtData> onSelected)
    {
        if (GodotObject.IsInstanceValid(_current))
            return;

        var popup = GD.Load<PackedScene>(ScenePath).Instantiate<NSelectCardPopup>();
        _current = popup;
        popup.TreeExiting += () => _current = null;
        parent.AddChild(popup);
        popup.Open(onSelected);
    }

    private ColorRect _dim = null!;
    private Label _title = null!;
    private Button _cancelButton = null!;
    private NSearchBar _searchBar = null!;
    private GridContainer _poolFilters = null!;

    private NCardViewSortButton _typeSorter = null!;
    private NCardTypeTickbox _attackFilter = null!;
    private NCardTypeTickbox _skillFilter = null!;
    private NCardTypeTickbox _powerFilter = null!;
    private NCardTypeTickbox _otherTypeFilter = null!;
    private readonly Dictionary<NCardTypeTickbox, Func<CardModel, bool>> _cardTypeFilters = [];

    private NCardViewSortButton _raritySorter = null!;
    private NCardRarityTickbox _commonFilter = null!;
    private NCardRarityTickbox _uncommonFilter = null!;
    private NCardRarityTickbox _rareFilter = null!;
    private NCardRarityTickbox _otherRarityFilter = null!;
    private readonly Dictionary<NCardRarityTickbox, Func<CardModel, bool>> _rarityFilters = [];

    private NCardViewSortButton _costSorter = null!;
    private NCardCostTickbox _zeroFilter = null!;
    private NCardCostTickbox _oneFilter = null!;
    private NCardCostTickbox _twoFilter = null!;
    private NCardCostTickbox _threePlusFilter = null!;
    private NCardCostTickbox _xFilter = null!;
    private readonly Dictionary<NCardCostTickbox, Func<CardModel, bool>> _costFilters = [];

    private NCardViewSortButton _alphabetSorter = null!;

    private MegaRichTextLabel _cardCountLabel = null!;
    private MegaRichTextLabel _noResultsLabel = null!;

    private Control _gridHost = null!;
    private NCardGrid _grid = null!;

    private readonly Dictionary<NCardPoolFilter, VotingPool> _poolFilterMap = [];
    private Dictionary<ModelId, ArtData> _byId = [];

    private readonly List<SortingOrders> _sortingPriority =
    [
        SortingOrders.RarityAscending,
        SortingOrders.TypeAscending,
        SortingOrders.CostAscending,
        SortingOrders.AlphabetAscending,
    ];

    private Action<ArtData>? _onSelected;

    public override void _Ready()
    {
        _dim = GetNode<ColorRect>("%Dim");
        _title = GetNode<Label>("%Title");
        _cancelButton = GetNode<Button>("%CancelButton");
        _searchBar = GetNode<NSearchBar>("%SearchBar");
        _poolFilters = GetNode<GridContainer>("%PoolFilters");

        _typeSorter = GetNode<NCardViewSortButton>("%TypeSorter");
        _attackFilter = GetNode<NCardTypeTickbox>("%AttackType");
        _skillFilter = GetNode<NCardTypeTickbox>("%SkillType");
        _powerFilter = GetNode<NCardTypeTickbox>("%PowerType");
        _otherTypeFilter = GetNode<NCardTypeTickbox>("%OtherType");

        _raritySorter = GetNode<NCardViewSortButton>("%RaritySorter");
        _commonFilter = GetNode<NCardRarityTickbox>("%CommonRarity");
        _uncommonFilter = GetNode<NCardRarityTickbox>("%UncommonRarity");
        _rareFilter = GetNode<NCardRarityTickbox>("%RareRarity");
        _otherRarityFilter = GetNode<NCardRarityTickbox>("%OtherRarity");

        _costSorter = GetNode<NCardViewSortButton>("%CostSorter");
        _zeroFilter = GetNode<NCardCostTickbox>("%Cost0");
        _oneFilter = GetNode<NCardCostTickbox>("%Cost1");
        _twoFilter = GetNode<NCardCostTickbox>("%Cost2");
        _threePlusFilter = GetNode<NCardCostTickbox>("%Cost3+");
        _xFilter = GetNode<NCardCostTickbox>("%CostX");

        _alphabetSorter = GetNode<NCardViewSortButton>("%AlphabetSorter");

        _cardCountLabel = GetNode<MegaRichTextLabel>("%CardCountLabel");
        _noResultsLabel = GetNode<MegaRichTextLabel>("%NoResultsLabel");
        _gridHost = GetNode<Control>("%GridHost");

        _title.Text = VotingUi.Loc("DOWNFALL-VOTING.select_card_title");
        _noResultsLabel.Text = new LocString("card_library", "NO_RESULTS").GetFormattedText();

        VotingUi.StyleActionButton(_cancelButton, primary: false);
        _cancelButton.Text = VotingUi.Loc("DOWNFALL-VOTING.cancel_button");
        _cancelButton.Pressed += QueueFree;

        _dim.GuiInput += e =>
        {
            if (e is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
                QueueFree();
        };

        _searchBar.Connect(NSearchBar.SignalName.QueryChanged, Callable.From<string>(_ => UpdateFilter()));
        _searchBar.Connect(NSearchBar.SignalName.QuerySubmitted, Callable.From<string>(_ => UpdateFilter()));

        _typeSorter.SetLabel(new LocString("gameplay_ui", "SORT_TYPE").GetRawText());
        _raritySorter.SetLabel(new LocString("gameplay_ui", "SORT_RARITY").GetRawText());
        _costSorter.SetLabel(new LocString("gameplay_ui", "SORT_COST").GetRawText());
        _alphabetSorter.SetLabel(new LocString("gameplay_ui", "SORT_ALPHABET").GetRawText());

        _typeSorter.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(_ => OnSort(_typeSorter, SortingOrders.TypeAscending, SortingOrders.TypeDescending)));
        _raritySorter.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(_ => OnSort(_raritySorter, SortingOrders.RarityAscending, SortingOrders.RarityDescending)));
        _costSorter.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(_ => OnSort(_costSorter, SortingOrders.CostAscending, SortingOrders.CostDescending)));
        _alphabetSorter.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(_ => OnSort(_alphabetSorter, SortingOrders.AlphabetAscending, SortingOrders.AlphabetDescending)));

        _commonFilter.SetLabel(new LocString("card_library", "RARITY_COMMON").GetRawText());
        _uncommonFilter.SetLabel(new LocString("card_library", "RARITY_UNCOMMON").GetRawText());
        _rareFilter.SetLabel(new LocString("card_library", "RARITY_RARE").GetRawText());
        _otherRarityFilter.SetLabel(new LocString("card_library", "RARITY_OTHER").GetRawText());

        _attackFilter.Loc = new LocString("card_library", "TYPE_ATTACK_TIP");
        _skillFilter.Loc = new LocString("card_library", "TYPE_SKILL_TIP");
        _powerFilter.Loc = new LocString("card_library", "TYPE_POWER_TIP");
        _otherTypeFilter.Loc = new LocString("card_library", "TYPE_OTHER_TIP");
        _commonFilter.Loc = new LocString("card_library", "RARITY_COMMON_TIP");
        _uncommonFilter.Loc = new LocString("card_library", "RARITY_UNCOMMON_TIP");
        _rareFilter.Loc = new LocString("card_library", "RARITY_RARE_TIP");
        _otherRarityFilter.Loc = new LocString("card_library", "RARITY_OTHER_TIP");
        _zeroFilter.Loc = new LocString("card_library", "COST_ZERO_TIP");
        _oneFilter.Loc = new LocString("card_library", "COST_ONE_TIP");
        _twoFilter.Loc = new LocString("card_library", "COST_TWO_TIP");
        _threePlusFilter.Loc = new LocString("card_library", "COST_THREE_TIP");
        _xFilter.Loc = new LocString("card_library", "COST_X_TIP");

        _cardTypeFilters.Add(_attackFilter, c => c.Type == CardType.Attack);
        _cardTypeFilters.Add(_skillFilter, c => c.Type == CardType.Skill);
        _cardTypeFilters.Add(_powerFilter, c => c.Type == CardType.Power);
        _cardTypeFilters.Add(_otherTypeFilter, c => c.Type is not (CardType.Attack or CardType.Skill or CardType.Power));
        foreach (var tickbox in _cardTypeFilters.Keys)
            tickbox.Connect(NCardTypeTickbox.SignalName.Toggled, Callable.From<NCardTypeTickbox>(_ => UpdateFilter()));

        _rarityFilters.Add(_commonFilter, c => c.Rarity == CardRarity.Common);
        _rarityFilters.Add(_uncommonFilter, c => c.Rarity == CardRarity.Uncommon);
        _rarityFilters.Add(_rareFilter, c => c.Rarity == CardRarity.Rare);
        _rarityFilters.Add(_otherRarityFilter, c => c.Rarity is not (CardRarity.Common or CardRarity.Uncommon or CardRarity.Rare));
        foreach (var tickbox in _rarityFilters.Keys)
            tickbox.Connect(NTickbox.SignalName.Toggled, Callable.From<NTickbox>(_ => UpdateFilter()));

        _costFilters.Add(_zeroFilter, c => c.EnergyCost is { Canonical: <= 0, CostsX: false });
        _costFilters.Add(_oneFilter, c => c.EnergyCost.Canonical == 1);
        _costFilters.Add(_twoFilter, c => c.EnergyCost.Canonical == 2);
        _costFilters.Add(_threePlusFilter, c => c.EnergyCost.Canonical >= 3);
        _costFilters.Add(_xFilter, c => c.EnergyCost.CostsX);
        foreach (var tickbox in _costFilters.Keys)
            tickbox.Connect(NClickableControl.SignalName.Released, Callable.From<NCardCostTickbox>(_ => UpdateFilter()));

        foreach (var tickbox in _cardTypeFilters.Keys) tickbox.IsTicked = false;
        foreach (var tickbox in _rarityFilters.Keys) tickbox.IsTicked = false;
        foreach (var tickbox in _costFilters.Keys) tickbox.IsTicked = false;
        _typeSorter.IsDescending = true;
        _raritySorter.IsDescending = true;
        _costSorter.IsDescending = true;
        _alphabetSorter.IsDescending = true;

        _grid = GD.Load<PackedScene>(CardGridScenePath).Instantiate<NCardGrid>();
        _grid.SetAnchorsPreset(LayoutPreset.FullRect);
        _gridHost.AddChild(_grid);
        _grid.Connect(NCardGrid.SignalName.HolderPressed, Callable.From<NCardHolder>(OnHolderPressed));
    }

    private void Open(Action<ArtData> onSelected)
    {
        _onSelected = onSelected;
        var categories = MissingArtCards.ComputeAll();
        _byId = categories.Where(c => c.Card != null).ToDictionary(c => c.Card!.Id);

        var pools = categories
            .Select(c => MissingArtCards.TryGetPool(c.Card, out var pool) ? pool : (VotingPool?)null)
            .Where(p => p != null).Select(p => p!.Value).Distinct().OrderBy(p => p.ToString());
        foreach (var pool in pools)
            AddPoolFilter(pool);

        // The grid's column count is derived from its ScrollContainer's
        // Size.X (see NCardGrid.Columns), which is still 0x0 the instant
        // this popup and its freshly-instantiated grid are added to the
        // tree - Godot's container layout pass hasn't run yet this frame.
        // Populating immediately makes NCardGrid build zero-width rows and
        // crash in AllocateCardHolders. Deferring one frame lets layout
        // settle first.
        Callable.From(UpdateFilter).CallDeferred();
    }

    private void AddPoolFilter(VotingPool pool)
    {
        var filter = GD.Load<PackedScene>(PoolFilterScenePath).Instantiate<NCardPoolFilter>();
        _poolFilters.AddChild(filter);
        _poolFilterMap.Add(filter, pool);

        var icon = NVoteCard.GetCharacterIcon(pool);
        if (icon != null)
            filter.GetNode<TextureRect>("Image").Texture = icon;

        filter.Connect(NCardPoolFilter.SignalName.Toggled, Callable.From<NCardPoolFilter>(f =>
        {
            if (f.IsSelected)
                foreach (var other in _poolFilterMap.Keys)
                    if (other != f)
                        other.IsSelected = false;
            UpdateFilter();
        }));
    }

    private void OnSort(NCardViewSortButton sorter, SortingOrders ascending, SortingOrders descending)
    {
        _sortingPriority.Remove(ascending);
        _sortingPriority.Remove(descending);
        _sortingPriority.Insert(0, sorter.IsDescending ? descending : ascending);
        DisplayCards();
    }

    private void UpdateFilter()
    {
        var activeTypeFilters = _cardTypeFilters.Where(kv => kv.Key.IsTicked).Select(kv => kv.Value).ToList();
        if (activeTypeFilters.Count == 0) activeTypeFilters.Add(_ => true);

        var activeRarityFilters = _rarityFilters.Where(kv => kv.Key.IsTicked).Select(kv => kv.Value).ToList();
        if (activeRarityFilters.Count == 0) activeRarityFilters.Add(_ => true);

        var activeCostFilters = _costFilters.Where(kv => kv.Key.IsTicked).Select(kv => kv.Value).ToList();
        if (activeCostFilters.Count == 0) activeCostFilters.Add(_ => true);

        var selectedPool = _poolFilterMap.FirstOrDefault(kv => kv.Key.IsSelected).Value;
        var poolActive = _poolFilterMap.Any(kv => kv.Key.IsSelected);

        var searchText = NSearchBar.Normalize(_searchBar.Text);

        bool TextFilter(CardModel card)
        {
            if (string.IsNullOrWhiteSpace(_searchBar.Text))
                return true;

            var text = card.Title + " " + NSearchBar.RemoveHtmlTags(card.GetDescriptionForPile(PileType.None).StripBbCode());
            return NSearchBar.Normalize(text).Contains(searchText);
        }

        bool Matches(CardModel card) =>
            (!poolActive || (MissingArtCards.TryGetPool(card, out var pool) && pool == selectedPool)) &&
            activeTypeFilters.Any(f => f(card)) &&
            activeRarityFilters.Any(f => f(card)) &&
            activeCostFilters.Any(f => f(card)) &&
            TextFilter(card);

        _cachedVisible = _byId.Values.Select(c => c.Card).Where(c => c != null && Matches(c)).Cast<CardModel>().ToList();

        DisplayCards();
    }

    private List<CardModel> _cachedVisible = [];
    private readonly LocString _cardCountLocString = new("card_library", "CARD_COUNT");

    private void DisplayCards()
    {
        _grid.SetCards(_cachedVisible, PileType.None, _sortingPriority);
        _cardCountLocString.Add("Amount", _cachedVisible.Count);
        _cardCountLabel.Text = "[center]" + _cardCountLocString.GetFormattedText() + "[/center]";
        _noResultsLabel.Visible = _cachedVisible.Count == 0;
    }

    private void OnHolderPressed(NCardHolder holder)
    {
        var card = holder.CardModel;
        if (card == null || !_byId.TryGetValue(card.Id, out var category))
            return;

        _onSelected?.Invoke(category);
        QueueFree();
    }
}
