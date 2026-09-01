using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;               // NButton, NClickableControl
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions; // NSearchBar, NCardViewSortButton
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;  // NCardPoolFilter

namespace Downfall.DownfallCode.Voting;

public partial class NVotingFilter : Control
{
    [Signal]
    public delegate void FilterChangedEventHandler();

    private enum SortMode { Likes, New, Alphabet }

    private NSearchBar _searchBar = null!;
    private readonly Dictionary<NCardPoolFilter, VotingPool> _pools = new();

    private NCardViewSortButton _likeSorter = null!;
    private NCardViewSortButton _newSorter = null!;
    private NCardViewSortButton _alphabetSorter = null!;
    private SortMode _activeSort = SortMode.Likes;

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

        _likeSorter     = RegisterSorter("%LikeSorter",     "Likes",    SortMode.Likes);
        _newSorter      = RegisterSorter("%NewSorter",      "New",      SortMode.New);
        _alphabetSorter = RegisterSorter("%AlphabetSorter", "Name",     SortMode.Alphabet);
    }

    private void RegisterPool(string path, VotingPool pool)
    {
        var filter = GetNode<NCardPoolFilter>(path);
        _pools[filter] = pool;
        filter.IsSelected = false;
        filter.Connect(NCardPoolFilter.SignalName.Toggled,
            Callable.From<NCardPoolFilter>(_ => EmitChanged()));
    }

    private NCardViewSortButton RegisterSorter(string path, string label, SortMode mode)
    {
        var sorter = GetNode<NCardViewSortButton>(path);
        sorter.SetLabel(label);
        sorter.Connect(NClickableControl.SignalName.Released,
            Callable.From<NButton>(_ => { _activeSort = mode; EmitChanged(); }));
        return sorter;
    }

    private void EmitChanged() => EmitSignal(SignalName.FilterChanged);

    // ---- The only surface NArtVotingScreen depends on ----

    public bool Matches(NVoteCard card)
    {
        var query = _searchBar.Text?.Trim() ?? string.Empty;
        if (query.Length > 0 &&
            !card.CardName.Contains(query, StringComparison.OrdinalIgnoreCase) &&
            !card.Author.Contains(query,   StringComparison.OrdinalIgnoreCase))
            return false;

        var selected = _pools.Where(kv => kv.Key.IsSelected)
                             .Select(kv => kv.Value)
                             .ToHashSet();
        if (selected.Count > 0 && !selected.Contains(card.Pool))
            return false;

        return true;
    }

    public List<NVoteCard> Sort(IEnumerable<NVoteCard> cards)
    {
        return _activeSort switch
        {
            SortMode.Likes => Directional(cards, _likeSorter, c => c.Likes)
                                .ThenBy(c => c.CardName, StringComparer.OrdinalIgnoreCase).ToList(),
            SortMode.New   => Directional(cards, _newSorter, c => c.SubmittedAt).ToList(),
            _              => Directional(cards, _alphabetSorter, c => c.CardName,
                                          StringComparer.OrdinalIgnoreCase).ToList(),
        };
    }

    private static IOrderedEnumerable<NVoteCard> Directional<TKey>(
        IEnumerable<NVoteCard> cards, NCardViewSortButton sorter,
        Func<NVoteCard, TKey> key, IComparer<TKey>? comparer = null)
    {
        return sorter.IsDescending
            ? cards.OrderByDescending(key, comparer)
            : cards.OrderBy(key, comparer);
    }
}

