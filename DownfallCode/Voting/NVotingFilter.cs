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

        RegisterPool("%AutomatonPool", VotingPool.Automaton);
        RegisterPool("%AwakenedPool",  VotingPool.Awakened);
        RegisterPool("%ChampPool",     VotingPool.Champ);
        RegisterPool("%GuardianPool",  VotingPool.Guardian);
        RegisterPool("%HermitPool",    VotingPool.Hermit);
        RegisterPool("%HexaghostPool", VotingPool.Hexaghost);
        RegisterPool("%SlimebossPool", VotingPool.Slimeboss);
        RegisterPool("%SneckoPool",    VotingPool.Snecko);

        RegisterSorter("%HotSorter",  VotingUi.Loc("DOWNFALL-VOTING.sort_hot"), SortMode.Hot);
        RegisterSorter("%LikeSorter", VotingUi.Loc("DOWNFALL-VOTING.sort_top"), SortMode.Top);
        RegisterSorter("%NewSorter",  VotingUi.Loc("DOWNFALL-VOTING.sort_new"), SortMode.New);
        _sorters[_activeSort].IsActive = true;
    }

    private void RegisterPool(string path, VotingPool pool)
    {
        var filter = GetNode<NCardPoolFilter>(path);
        _pools[filter] = pool;
        filter.IsSelected = false;
        filter.Connect(NCardPoolFilter.SignalName.Toggled,
            Callable.From<NCardPoolFilter>(_ => EmitChanged()));
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

