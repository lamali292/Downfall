using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.Cards;
// TaskHelper
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.TestSupport;

namespace Downfall.DownfallCode.Voting;

/// <summary>
/// Shows the public voting grid for cards missing art. Uploading and
/// browsing your own submissions are separate, self-contained popups
/// (<see cref="NUploadArtPopup"/>, <see cref="NMySubmissionsPopup"/>) that
/// this screen merely opens - it doesn't own their wiring, so any other
/// screen can open them the same way without depending on this class.
///
/// Loading is a paginated, server-sorted feed, not "fetch every submission
/// for every missing-art card up front": that doesn't scale (every card's
/// full history, downloaded and instantiated as a node, the moment the
/// screen opens), and computing "hot" client-side over a partial page
/// wouldn't rank correctly anyway. Each (sort, pool-set) combination gets
/// its own page cache keyed in <see cref="_cache"/>, so switching sort or
/// pool back and forth re-renders instantly from cache instead of
/// re-fetching, and scrolling near the bottom fetches the next page of
/// whichever feed is active. The search box stays a client-side filter over
/// whatever's already loaded - it doesn't affect what gets fetched, so a
/// search term can miss matches sitting in a page that hasn't been scrolled
/// to yet.
/// </summary>
public partial class NArtVotingScreen : NSubmenu
{
    private const string ScenePath = "res://Downfall/scenes/voting/voting.tscn";
    private const string VoteCardScenePath = "res://Downfall/scenes/voting/art_row.tscn";
    private const string MusicEvent = "event:/music/downfall_menu";

    // This screen is only ever reached from the main menu (see
    // DownfallMainFile's MainMenuButtonRegistry entry), whose own music is
    // already playing underneath when this screen opens - StopMusic() on
    // close would kill that too and leave the main menu silent, since
    // NMainMenu only (re-)starts its music from its own OnSubmenuOpened,
    // which doesn't fire again for the screen underneath a popped submenu.
    // Resume it explicitly instead of a blanket stop.
    private const string MainMenuMusicEvent = "event:/music/menu_update";
    private const int PageSize = 30;

    // How close to the bottom (0-100, matches NScrollbar.Value) triggers
    // loading the next page.
    private const double LoadMoreThreshold = 75;

    protected override Control? InitialFocusedControl => null;

    public static NArtVotingScreen? Create() =>
        TestMode.IsOn ? null : PreloadManager.Cache.GetScene(ScenePath).Instantiate<NArtVotingScreen>();

    private sealed class FeedCache
    {
        public required IReadOnlySet<VotingPool> Pools;
        public readonly List<ArtEntry> Items = [];
        public int? NextOffset = 0;
        public bool Loading;
    }

    private NVotingFilter _filter = null!;
    private HFlowContainer _content = null!;
    private NScrollableContainer _scroll = null!;
    private Control? _mask;
    private PackedScene _voteCardScene = null!;
    private bool _loaded;

    private readonly Dictionary<(NVotingFilter.SortMode Sort, string PoolKey), FeedCache> _cache = new();
    private (NVotingFilter.SortMode Sort, string PoolKey) _activeKey;

    public override void _Ready()
    {
        ConnectSignals();

        _filter  = GetNode<NVotingFilter>("%VotingFilter");
        _content = GetNode<HFlowContainer>("%Content");
        _scroll  = GetNodeOrNull<NScrollableContainer>("%ScrollView");
        _voteCardScene = GD.Load<PackedScene>(VoteCardScenePath);

        // Wired explicitly (not just relying on NScrollableContainer's own
        // "Content"/"Mask/Content" auto-detection) so this keeps working
        // regardless of exactly how voting.tscn nests things under %ScrollView.
        _scroll?.SetContent(_content);
        _filter.FilterChanged += OnFilterChanged;
        _scroll?.Scrollbar.Connect(Godot.Range.SignalName.ValueChanged, Callable.From<double>(OnScrolled));

        // HFlowContainer's own reported Size.X consistently overshoots its
        // anchor-derived rect by a fixed amount (its wrap decision fits one
        // column too many, regardless of Mask's actual width), which without
        // grow_horizontal pinned to End would spill out both edges of Mask's
        // clip. Clamp its width back to Mask's own width and force a re-sort
        // against that clamp, both right after content changes and whenever
        // Mask resizes.
        _mask = _scroll?.GetNodeOrNull<Control>("Mask");
        _mask?.Connect(Control.SignalName.Resized,
            Callable.From(() => Callable.From(ClampContentWidth).CallDeferred()));

        foreach (var child in _content.GetChildren())
            child.QueueFree();

        AddHeaderButtons();
    }

    public override void _ExitTree()
    {
        _filter.FilterChanged -= OnFilterChanged;
    }

    public override void OnSubmenuOpened()
    {
        NAudioManager.Instance?.PlayMusic(MusicEvent);

        if (!_loaded)
        {
            _loaded = true;
            _activeKey = CurrentKey();
            SwitchFeed();
        }
        else
        {
            _scroll?.InstantlyScrollToTop();
        }
    }

    public override void OnSubmenuClosed()
    {
        base.OnSubmenuClosed();
        NAudioManager.Instance?.PlayMusic(MainMenuMusicEvent);
    }

    // ---- Feed loading (paginated, cached per sort+pool combo) ----

    private (NVotingFilter.SortMode Sort, string PoolKey) CurrentKey() =>
        (_filter.ActiveSort, PoolKey(_filter.SelectedPools));

    private static string PoolKey(IReadOnlySet<VotingPool> pools) =>
        pools.Count == 0 ? "" : string.Join(",", pools.OrderBy(p => p));

    private FeedCache GetOrCreateCache((NVotingFilter.SortMode Sort, string PoolKey) key)
    {
        if (_cache.TryGetValue(key, out var cache))
            return cache;

        cache = new FeedCache { Pools = _filter.SelectedPools };
        _cache[key] = cache;
        return cache;
    }

    private void OnFilterChanged()
    {
        var newKey = CurrentKey();
        if (newKey != _activeKey)
        {
            _activeKey = newKey;
            SwitchFeed();
        }
        else
        {
            ApplyLocalFilter();
        }
    }

    /// <summary>
    /// Renders <see cref="_activeKey"/>'s cache from scratch - instantly if
    /// it's already (fully or partially) loaded, otherwise kicks off the
    /// first page fetch.
    /// </summary>
    private void SwitchFeed()
    {
        foreach (var child in _content.GetChildren())
            child.QueueFree();

        var cache = GetOrCreateCache(_activeKey);

        if (cache.Items.Count > 0)
        {
            foreach (var entry in cache.Items)
                AddCard(entry);

            ApplyLocalFilter();
            Callable.From(() => { ClampContentWidth(); EnsureFilled(); }).CallDeferred();
        }
        else if (cache.NextOffset != null && !cache.Loading)
        {
            TaskHelper.RunSafely(FetchPage(_activeKey, cache));
        }

        _scroll?.InstantlyScrollToTop();
    }

    private void OnScrolled(double value)
    {
        if (value < LoadMoreThreshold)
            return;

        if (!_cache.TryGetValue(_activeKey, out var cache) || cache.Loading || cache.NextOffset == null)
            return;

        TaskHelper.RunSafely(FetchPage(_activeKey, cache));
    }

    /// <summary>
    /// A short feed (or one heavily thinned by the search box) may not fill
    /// the viewport, so there's no scrollbar to trigger <see cref="OnScrolled"/>
    /// - top it up automatically instead of leaving the user stuck.
    /// </summary>
    private void EnsureFilled()
    {
        if (!IsInstanceValid(this) || _scroll == null || _scroll.Scrollbar.Visible)
            return;

        if (!_cache.TryGetValue(_activeKey, out var cache) || cache.Loading || cache.NextOffset == null)
            return;

        TaskHelper.RunSafely(FetchPage(_activeKey, cache));
    }

    private async Task FetchPage((NVotingFilter.SortMode Sort, string PoolKey) key, FeedCache cache)
    {
        if (cache.Loading || cache.NextOffset == null)
            return;

        cache.Loading = true;

        var sort = key.Sort switch
        {
            NVotingFilter.SortMode.Top => "top",
            NVotingFilter.SortMode.New => "new",
            _ => "hot",
        };

        var result = await VotingApi.Instance.GetSubmissionsFeed(cache.Pools, sort, cache.NextOffset.Value, PageSize);

        cache.Loading = false;

        // The screen may have closed, or the user may have switched to a
        // different sort/pool, while this request was in flight - either
        // way this page no longer belongs anywhere.
        if (!IsInstanceValid(this) || _activeKey != key)
            return;

        if (result == null)
        {
            cache.NextOffset = null;
            return;
        }

        var (items, nextOffset) = result.Value;
        cache.NextOffset = nextOffset;
        cache.Items.AddRange(items);

        foreach (var entry in items)
            AddCard(entry);

        ApplyLocalFilter();
        Callable.From(() => { ClampContentWidth(); EnsureFilled(); }).CallDeferred();
    }

    /// <summary>
    /// HFlowContainer's own <c>Size.X</c> here consistently overshoots
    /// Mask's width by a fixed amount, however many columns it wraps to -
    /// pull it back so Mask's clip lines up with a whole number of columns
    /// instead of slicing through the last one. Re-sorting against the
    /// clamped size is what actually removes the extra column, not just the
    /// clamp itself.
    /// </summary>
    private void ClampContentWidth()
    {
        if (_mask == null || !IsInstanceValid(this))
            return;

        if (!Mathf.IsEqualApprox(_content.Size.X, _mask.Size.X))
        {
            _content.Size = new Vector2(_mask.Size.X, _content.Size.Y);
            _content.QueueSort();
        }
    }

    private void AddCard(ArtEntry entry)
    {
        var card = _voteCardScene.Instantiate<NVoteCard>();
        _content.AddChild(card);
        card.Pool = MissingArtCards.PoolFor(entry.Card);
        card.PopupHost = this;
        card.SetEntry(entry);
        card.CardClicked += OnCardClicked;
        card.Connect(NVoteCard.SignalName.ScoreChanged, Callable.From(() => OnCardScoreChanged(card)));
    }

    /// <summary>
    /// A vote card's own like/count state lives on the node (see
    /// <see cref="NVoteCard.Like"/>) - the <see cref="ArtEntry"/> sitting in
    /// a <see cref="FeedCache"/> is a separate, immutable snapshot from
    /// whenever that page was fetched. Without this, liking a card, then
    /// switching sort/pool and back, re-renders from the stale cached entry
    /// and the like visually reverts even though the server still has it.
    /// The same submission can appear as a separate ArtEntry instance in
    /// several caches at once (Hot/Top/New, per pool filter), so every
    /// cache gets checked, not just the active one.
    /// </summary>
    private void OnCardScoreChanged(NVoteCard card)
    {
        foreach (var cache in _cache.Values)
        {
            for (var i = 0; i < cache.Items.Count; i++)
            {
                if (cache.Items[i].Id != card.SubmissionId)
                    continue;

                cache.Items[i] = cache.Items[i] with { Liked = card.Liked, Upvotes = card.Likes };
                break;
            }
        }
    }

    private void ApplyLocalFilter()
    {
        foreach (var card in _content.GetChildren().OfType<NVoteCard>())
            card.Visible = _filter.MatchesSearch(card);
    }

    // ---- Card art preview ----

    private async void OnCardClicked(string imagePath, string category, string entry)
    {
        if (!NVoteCard.TextureCache.TryGetValue(imagePath, out var tex))
            return;

        var card = ModelDb.GetByIdOrNull<CardModel>(
            new ModelId(category, entry));

        if (card == null)
            return;

        var rect = NCard.Create(card);
        if (rect == null)
            return;

        var overlay = new ColorRect
        {
            Color = new Color(0, 0, 0, 0.85f),
            MouseFilter = MouseFilterEnum.Stop
        };
        overlay.SetAnchorsPreset(LayoutPreset.FullRect);

        var center = new CenterContainer();
        center.SetAnchorsPreset(LayoutPreset.FullRect);

        AddChild(overlay);
        overlay.AddChild(center);
        center.AddChild(rect);

        if (!rect.IsNodeReady())
            await ToSignal(rect, Node.SignalName.Ready);

        rect.UpdateVisuals(PileType.Deck, CardPreviewMode.Normal);

        if (card.Rarity == CardRarity.Ancient)
            rect._ancientPortrait.Texture = tex;
        else
            rect._portrait.Texture = tex;

        overlay.GuiInput += e =>
        {
            if (e is InputEventMouseButton
                {
                    Pressed: true,
                    ButtonIndex: MouseButton.Left
                })
            {
                overlay.QueueFree();
            }
        };
    }

    // ---- Header buttons ----
    // Just opens the popups; doesn't manage their internals. Either popup
    // could just as easily be opened from a totally different screen.

    private void AddHeaderButtons()
    {
        var upload = CreateHeaderButton(VotingUi.Loc("DOWNFALL-VOTING.upload_button"), -HeaderButtonGap / 2f - HeaderButtonWidth);
        upload.Pressed += () => NUploadArtPopup.OpenFrom(this, _ => OnArtUploaded());
        AddChild(upload);

        var mySubmissions = CreateHeaderButton(VotingUi.Loc("DOWNFALL-VOTING.my_submissions_button"), HeaderButtonGap / 2f);
        mySubmissions.Pressed += () => NMySubmissionsPopup.OpenFrom(this);
        AddChild(mySubmissions);
    }

    /// <summary>
    /// A fresh upload starts out pending review, so it won't appear in any
    /// feed yet - this is a "just in case something changed" hard refresh
    /// rather than something expected to surface the new submission itself.
    /// </summary>
    private void OnArtUploaded()
    {
        _cache.Clear();
        SwitchFeed();
    }

    // Sized up from VotingUi's default 150x46 action button and anchored to
    // top-center (as a pair straddling the midline) rather than top-right, so
    // Submit/My Submissions read as the screen's primary calls-to-action -
    // matching the scale the base game gives its own top-center header
    // buttons - instead of a secondary corner utility.
    // OffsetTop=20, OffsetBottom=20+HeaderButtonHeight below, so these buttons
    // occupy the top ~80px of the screen - voting.tscn's MarginContainer2
    // (holding the card grid) has margin_top=100 to clear that instead of the
    // grid starting underneath the buttons; keep both in sync if this changes.
    private const float HeaderButtonWidth = 220f;
    private const float HeaderButtonHeight = 60f;
    private const float HeaderButtonGap = 20f;

    private static Button CreateHeaderButton(string text, float offsetLeft)
    {
        var button = VotingUi.CreateActionButton(text, primary: true);
        button.CustomMinimumSize = new Vector2(HeaderButtonWidth, HeaderButtonHeight);
        button.AddThemeFontSizeOverride("font_size", 24);
        button.LayoutMode = 1;
        button.AnchorLeft = 0.5f;
        button.AnchorRight = 0.5f;
        button.AnchorTop = 0f;
        button.AnchorBottom = 0f;
        button.OffsetLeft = offsetLeft;
        button.OffsetRight = offsetLeft + HeaderButtonWidth;
        button.OffsetTop = 20f;
        button.OffsetBottom = 20f + HeaderButtonHeight;
        return button;
    }
}
