using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Piles;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Automaton.AutomatonCode.Vfx;

[GlobalClass]
public partial class NStashDisplay : NSlotRevealDisplay
{
    private const float StashDisplayScale = 0.28f;
    protected override float SlotSeparation => -100f;
    protected override float PreviewGap => 0f;
    private const string DisplayScenePath = "res://Automaton/scenes/ui/stash_display.tscn";

    private static readonly Dictionary<Player, NStashDisplay> Displays = new();

    private CombatManager? _combatManager;
    private CardPile? _stashPile;
    private Player? _trackedPlayer;

    static NStashDisplay()
    {
        CombatManager.Instance.CombatEnded += _ =>
        {
            foreach (var d in Displays.Values.Where(IsInstanceValid))
                d.QueueFree();
            Displays.Clear();
        };
    }

    // --- Base overrides: presentation config ---

    // Next-draw card is a normal card, not a big compiled Function.
    protected override float PreviewCardScale => 1.0f;
    

    protected override bool IsActive =>
        _trackedPlayer != null && _combatManager is { IsInProgress: true };

    // --- Base overrides: data ---

    /// <summary>Oldest card (drawn next) lives in the preview; the slots hold the rest.</summary>
    protected override IReadOnlyList<CardModel> GetSlotCards()
    {
        return _stashPile?.Cards.Skip(1).Reverse().ToList() ?? [];
    }

    /// <summary>Exactly as many slots as there are cards beyond the preview (0..4).</summary>
    protected override int GetMaxSlots()
    {
        var count = _stashPile?.Cards.Count ?? 0;
        return count - 1;
    }

    protected override CardModel? CreatePreviewModel(IReadOnlyList<CardModel> slotCards)
    {
        return _stashPile?.Cards.FirstOrDefault();
    }

    /// <summary>
    /// The badge and preview depend on the whole pile, not just the slot row —
    /// without this, adding the first card wouldn't register as a change
    /// (slot row stays empty, max stays 0) and the display would stick at 0/5.
    /// </summary>
    protected override IReadOnlyList<CardModel> GetDirtyCheckCards()
    {
        return _stashPile?.Cards.ToList() ?? [];
    }

    protected override string BuildCountText(IReadOnlyList<CardModel> slotCards)
    {
        return $"{_stashPile?.Cards.Count ?? 0}/{StashCmd.MaxStashSize}";
    }

    /// <summary>Inspect in draw order: pile front (next draw) first.</summary>
    protected override List<CardModel> BuildInspectList()
    {
        return _stashPile?.Cards.Reverse().ToList() ?? [];
    }

    // --- Public surface (matches the old StashQueueDisplay) ---

    public static NStashDisplay? GetDisplay(Player player)
    {
        return Displays.GetValueOrDefault(player);
    }

    public int GetQueueCount()
    {
        return _stashPile?.Cards.Count ?? 0;
    }

    public int GetCardIndex(CardModel card)
    {
        return _stashPile?.Cards.IndexOf(card) ?? -1;
    }

    /// <summary>
    /// Global target position for a card flying into the stash.
    /// Pile index 0 = the preview slot; index N = slot N-1.
    /// </summary>
    public Vector2 GetCardGlobalPosition(int pileIndex)
    {
        if (pileIndex <= 0)
            return PreviewSlot?.CardAnchorGlobal ?? GlobalPosition;
        return GetSlotGlobalPosition(pileIndex - 1);
    }
    
    public static bool HasDisplay(Player player)
    {
        var display = Displays.GetValueOrDefault(player);
        return display != null && IsInstanceValid(display);
    }

/*
    public static void SetupFor(NCombatRoom combatRoom, Player player)
    {
        if (HasDisplay(player)) return;
        var scene = ResourceLoader.Load<PackedScene>(DisplayScenePath);
        var display = scene.Instantiate<NStashDisplay>();
        display._trackedPlayer = player;
        display.Scale = Vector2.One * (LocalContext.IsMe(player) ? StashDisplayScale : StashDisplayScale * 0.5f);
        display.Direction = RevealDirection.Left;
        display.ZIndex = LocalContext.IsMe(player) ? 1 : 0;   
        var vfxContainer = combatRoom.CombatVfxContainer;
        vfxContainer.AddChildSafely(display);

        var creatureNode = combatRoom.GetCreatureNode(player.Creature);
        if (creatureNode != null)
        {
            var globalTopPos = creatureNode.GetTopOfHitbox();
            var localPos = vfxContainer.GetGlobalTransform().AffineInverse() * globalTopPos;
            var x = LocalContext.IsMe(player) ? -90 : -50;
            var y = LocalContext.IsMe(player) ? -100 : -40;
            display.Position = localPos + new Vector2(x, y); 
        }

        Displays[player] = display;
        display.SubscribeToStash(player);
        display.Refresh(true);
    }
    */

    
    public static void SetupFor(NCombatRoom combatRoom, Player player)
    {
        if (!LocalContext.IsMe(player) || HasDisplay(player)) return;
        var energyNode = combatRoom.Ui._energyCounter; // see note on the type below
        var display = ResourceLoader.Load<PackedScene>(DisplayScenePath).Instantiate<NStashDisplay>();
        display._trackedPlayer = player;
        display.Direction = RevealDirection.Right;
        display.Scale = Vector2.One * StashDisplayScale;

        energyNode.AddChildSafely(display);
        display.Position = energyNode.Position + new Vector2(70, -120); // tune offset

        Displays[player] = display;
        display.SubscribeToStash(player);
        display.Refresh(true);
    }
    
    /// <summary>Create on demand (e.g. from the Stash keyword's command) if not present.</summary>
    public static void EnsureFor(Player player)
    {
        if (HasDisplay(player)) return;
        var combatRoom = NCombatRoom.Instance;
        if (combatRoom != null)
            SetupFor(combatRoom, player);
    }
    // --- Lifecycle / pile subscription ---

    public override void _Ready()
    {
        base._Ready();
        _combatManager = CombatManager.Instance;
    }

    private void SubscribeToStash(Player player)
    {
        _stashPile = CustomPiles.GetCustomPile(player.PlayerCombatState, StashPile.Stash);
        if (_stashPile == null) return;
        _stashPile.CardAdded += OnPileChanged;
        _stashPile.CardRemoved += OnPileChanged;
    }

    private void OnPileChanged(CardModel _)
    {
        Refresh();
    }

    public override void _ExitTree()
    {
        if (_stashPile == null) return;
        _stashPile.CardAdded -= OnPileChanged;
        _stashPile.CardRemoved -= OnPileChanged;
        _stashPile = null;
    }
}