using Godot;
using MegaCrit.Sts2.Core.Nodes.CommonUi; // NSearchBar
using MegaCrit.Sts2.Core.Nodes.GodotExtensions; // NClickableControl
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;  // NCardPoolFilter

namespace Downfall.DownfallCode.Voting;

public partial class NVotingFilter : Control
{
    [Signal]
    public delegate void FilterChangedEventHandler();

    public enum SortMode { Top, New, Hot }

    private const string PoolToggleScenePath = "res://scenes/screens/card_library/library_pool_toggle.tscn";

    private NSearchBar _searchBar = null!;
    private readonly Dictionary<NCardPoolFilter, VotingPool> _pools = new();

    private readonly Dictionary<SortMode, NVotingSortButton> _sorters = new();
    private SortMode _activeSort = SortMode.Hot;

    public override void _Ready()
    {
        _searchBar = GetNode<NSearchBar>("%SearchBar");
        _searchBar.Connect(NSearchBar.SignalName.QueryChanged,
            Callable.From<string>(_ => EmitChanged()));
        _searchBar.Connect(NSearchBar.SignalName.QuerySubmitted,
            Callable.From<string>(_ => EmitChanged()));

        BuildPoolFilters();

        RegisterSorter("%HotSorter",  VotingUi.Loc("DOWNFALL-VOTING.sort_hot"), SortMode.Hot);
        RegisterSorter("%LikeSorter", VotingUi.Loc("DOWNFALL-VOTING.sort_top"), SortMode.Top);
        RegisterSorter("%NewSorter",  VotingUi.Loc("DOWNFALL-VOTING.sort_new"), SortMode.New);
        _sorters[_activeSort].IsActive = true;
    }

    /// <summary>
    /// One toggle button per pool that registered with <see cref="VotingPoolRegistry"/>,
    /// instantiated from the base game's own pool-toggle scene instead of relying on the
    /// filter's own .tscn to list every character by name - a submod that isn't in the
    /// build simply never registers, and a new one needs no scene edits here.
    /// </summary>
    private void BuildPoolFilters()
    {
        var container = GetNode<GridContainer>("%PoolFilters");
        var toggleScene = GD.Load<PackedScene>(PoolToggleScenePath);

        foreach (var pool in VotingPoolRegistry.RegisteredPools)
        {
            var iconPath = VotingPoolRegistry.IconPath(pool);
            var icon = iconPath != null && ResourceLoader.Exists(iconPath)
                ? GD.Load<Texture2D>(iconPath)
                : null;

            if (icon == null)
                continue;

            var filter = toggleScene.Instantiate<NCardPoolFilter>();
            filter.Name = $"{pool}Pool";
            container.AddChild(filter);

            foreach (var rect in filter.FindChildren("*", nameof(TextureRect), true, false)
                                        .OfType<TextureRect>())
            {
                rect.Texture = icon;
            }

            _pools[filter] = pool;
            filter.IsSelected = false;
            filter.Connect(NCardPoolFilter.SignalName.Toggled,
                Callable.From<NCardPoolFilter>(_ => EmitChanged()));
        }
    }

    /// <summary>
    /// Single-state switches: clicking one selects it as the active sort
    /// (always "best first" - highest votes / newest / hottest) and
    /// highlights it; clicking the already-active one is a no-op, not a
    /// flip. See <see cref="NVotingSortButton"/> for why this isn't the
    /// vanilla card-library sort button.
    /// </summary>
    private void RegisterSorter(string path, string label, SortMode mode)
    {
        var sorter = GetNode<NVotingSortButton>(path);
        sorter.SetLabel(label);
        _sorters[mode] = sorter;
        sorter.Connect(NClickableControl.SignalName.Released,
            Callable.From<NVotingSortButton>(_ => Select(mode)));
    }

    private void Select(SortMode mode)
    {
        if (_activeSort == mode)
            return;

        _sorters[_activeSort].IsActive = false;
        _activeSort = mode;
        _sorters[_activeSort].IsActive = true;
        EmitChanged();
    }

    private void EmitChanged() => EmitSignal(SignalName.FilterChanged);

    // ---- The only surface NArtVotingScreen depends on ----

    /// <summary>
    /// Which sort is active right now. Sorting and pool scoping both happen
    /// server-side (see <see cref="NArtVotingScreen"/>'s feed cache) so this
    /// and <see cref="SelectedPools"/> are what the screen keys its
    /// per-(sort, pool set) page cache on - only the search box stays a
    /// purely local filter over whatever's already loaded.
    /// </summary>
    public SortMode ActiveSort => _activeSort;

    /// <summary>Empty means "no pool filter" (every pool matches).</summary>
    public IReadOnlySet<VotingPool> SelectedPools =>
        _pools.Where(kv => kv.Key.IsSelected).Select(kv => kv.Value).ToHashSet();

    public bool MatchesSearch(NVoteCard card)
    {
        var query = _searchBar.Text?.Trim() ?? string.Empty;
        return query.Length == 0 ||
               card.CardName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
               card.Author.Contains(query, StringComparison.OrdinalIgnoreCase);
    }
}

