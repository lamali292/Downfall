using Automaton.AutomatonCode.Core;
using Awakened.AwakenedCode.Core;
using Champ.ChampCode.Core;
using Godot;
using Guardian.GuardianCode.Core;
using Hermit.HermitCode.Core;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
// TaskHelper
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;  
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;  
using MegaCrit.Sts2.Core.TestSupport;
using SlimeBoss.SlimeBossCode.Core;
using Snecko.SneckoCode.Core;

namespace Downfall.DownfallCode.Voting;

public partial class NArtVotingScreen : NSubmenu
{
    private const string ScenePath = "res://Downfall/scenes/voting/voting.tscn";
    private const string VoteCardScenePath = "res://Downfall/scenes/voting/art_row.tscn";

    protected override Control? InitialFocusedControl => null;

    public static NArtVotingScreen? Create() =>
        TestMode.IsOn ? null : PreloadManager.Cache.GetScene(ScenePath).Instantiate<NArtVotingScreen>();

    private NVotingFilter _filter = null!;
    private HFlowContainer _content = null!;
    private NScrollableContainer _scroll  = null!;
    private PackedScene _voteCardScene = null!;
    private bool _loaded;
    
    public override void _Ready()
    {
        ConnectSignals();

        _filter  = GetNode<NVotingFilter>("%VotingFilter");
        _content = GetNode<HFlowContainer>("%Content");
        _scroll  = GetNodeOrNull<NScrollableContainer>("%ScrollView");
        _voteCardScene = GD.Load<PackedScene>(VoteCardScenePath);

        _filter.FilterChanged += ApplyFilter;

        foreach (var child in _content.GetChildren())
            child.QueueFree();
    }

    public override void _ExitTree()
    {
        _filter.FilterChanged -= ApplyFilter;
    }
    public override void OnSubmenuOpened()
    {
        if (!_loaded)
        {
            _loaded = true;
            TaskHelper.RunSafely(LoadAll());
        }
        else
        {
            _scroll?.InstantlyScrollToTop();
        }
    }

    private async Task LoadAll()
    {
        var categories = await VotingApi.Instance.GetCategories();
        if (categories == null)
        {
            GD.PrintErr("Failed to load voting categories");
            return;
        }

        // Fetch every category's submissions in parallel; results keep input order.
        var jobs = categories.Select(async cat =>
            (cat, entries: await VotingApi.Instance.GetSubmissions(cat)));

        if (!IsInstanceValid(this))
            return;

        foreach (var (cat, entries) in await Task.WhenAll(jobs))
        {
            if (entries == null)
                continue;
            var pool = PoolFor(cat);
            foreach (var entry in entries)
                AddCard(entry, pool);
        }

        ApplyFilter();
    }

    private void AddCard(ArtEntry entry, VotingPool pool)
    {
        var card = _voteCardScene.Instantiate<NVoteCard>();
        _content.AddChild(card);
        card.Pool = pool;   
        card.SetEntry(entry);
        card.CardClicked += OnCardClicked;
    }

    
    private static VotingPool PoolFor(ArtData category)
    {
        return category.Card?.Pool switch
        {
            AutomatonCardPool => VotingPool.Automaton,
            AwakenedCardPool  => VotingPool.Awakened,
            ChampCardPool     => VotingPool.Champ,
            GuardianCardPool  => VotingPool.Guardian,
            HermitCardPool    => VotingPool.Hermit,
            HexaghostCardPool => VotingPool.Hexaghost,
            SlimeBossCardPool => VotingPool.Slimeboss,
            SneckoCardPool    => VotingPool.Snecko,
            _ => VotingPool.Automaton,
        };
    }
    // ---- Filtering / ordering ----

    private void ApplyFilter()
    {
        var cards = _content.GetChildren().OfType<NVoteCard>().ToList();

        foreach (var card in cards)
            card.Visible = _filter.Matches(card);   // hidden cards leave no gaps in HFlow

        var ordered = _filter.Sort(cards);
        for (var i = 0; i < ordered.Count; i++)
            _content.MoveChild(ordered[i], i);

        _scroll?.InstantlyScrollToTop();
    }



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
}