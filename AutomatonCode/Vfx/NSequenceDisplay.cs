using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Events;
using Automaton.AutomatonCode.Extensions;
using Automaton.AutomatonCode.Piles;
using BaseLib.Patches.Content;
using Downfall.DownfallCode.Nodes;
using Downfall.DownfallCode.Patches;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Automaton.AutomatonCode.Vfx;

[GlobalClass]
public partial class NSequenceDisplay : NSlotRevealDisplay
{
    private const float SequencedCardScale = 0.28f;
    private const string DisplayScenePath = "res://Automaton/scenes/ui/automaton_display.tscn";

    private CombatManager? _combatManager;
    private Player? _trackedPlayer;

    protected override bool IsActive =>
        _trackedPlayer != null && _combatManager is { IsInProgress: true };
    
    public override void _Ready()
    {
        base._Ready();
        _combatManager = CombatManager.Instance;
    }

    protected override IReadOnlyList<CardModel> GetSlotCards()
    {
        return _trackedPlayer?.GetEncode() ?? [];
    }

    protected override int GetMaxSlots()
    {
        return _trackedPlayer == null ? 3 : AutomatonCmd.GetMax(_trackedPlayer);
    }

    protected override CardModel? CreatePreviewModel(IReadOnlyList<CardModel> slotCards)
    {
        if (_trackedPlayer == null) return null;
        if (ModelDb.Card<FunctionCard>().ToMutable() is not FunctionCard model) return null;

        if (slotCards.Count > 0) model.SetSourceCards(slotCards);
        model.Owner = _trackedPlayer;
        return AutomatonHook.ModifyCompiledFunction(_trackedPlayer.Creature.CombatState!, model,
            _trackedPlayer, out _);
    }

    protected override void OnSlotCardSet(int index, CardModel model, NCard node, NCustomCardHolder holder)
    {
        FindOnTablePatch.Register(model, node);
    }

    protected override void OnSlotCardCleared(CardModel model)
    {
        FindOnTablePatch.Unregister(model);
    }

    protected override List<CardModel> BuildInspectList()
    {
        var list = (CustomPiles.GetCustomPile(_trackedPlayer?.PlayerCombatState, EncodePile.FunctionSequence)?.Cards ?? [])
            .Concat(CardHolders.Where(h => h.CardModel != null).Select(h => h.CardModel!)).ToList();
        if (PreviewModel != null) list.Add(PreviewModel);
        return list;
    }
    
    private static readonly Dictionary<Player, NSequenceDisplay> Displays = new();

    static NSequenceDisplay()
    {
        CombatManager.Instance.CombatEnded += _ =>
        {
            foreach (var d in Displays.Values.Where(IsInstanceValid))
                d.QueueFree();
            Displays.Clear();
        };
    }

    public static NSequenceDisplay? GetDisplay(Player player)
    {
        return Displays.GetValueOrDefault(player);
    }

    public static void SetupFor(NCombatRoom combatRoom, Player player)
    {
        var scene = ResourceLoader.Load<PackedScene>(DisplayScenePath);
        var display = scene.Instantiate<NSequenceDisplay>();
        display._trackedPlayer = player;
        display.Scale = Vector2.One * (LocalContext.IsMe(player) ? SequencedCardScale : SequencedCardScale * 0.5f);
        display.Direction = RevealDirection.Right;
        display.ZIndex = LocalContext.IsMe(player) ? 1 : 0;  
        var vfxContainer = combatRoom.CombatVfxContainer;
        vfxContainer.AddChildSafely(display);

        // Position it wherever SetupAutomatonUi used to put it — e.g. above the player:
        var creatureNode = combatRoom.GetCreatureNode(player.Creature);
        if (creatureNode != null)
        {
            var globalTopPos = creatureNode.GetTopOfHitbox();
            var localPos = vfxContainer.GetGlobalTransform().AffineInverse() * globalTopPos;
            var x = LocalContext.IsMe(player) ? 90 : 50;
            var y = LocalContext.IsMe(player) ? -100 : -40;
            display.Position = localPos + new Vector2(x, y); 
        }

        Displays[player] = display;
        display.Refresh(true);
    }

    /// <summary>Static refresh used by game logic (AutomatonCmd etc.).</summary>
    public static void Refresh(Player player, bool force = false)
    {
        var display = GetDisplay(player);
        if (display != null && IsInstanceValid(display))
            display.Refresh(force);
    }
}