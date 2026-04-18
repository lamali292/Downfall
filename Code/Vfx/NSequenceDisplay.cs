using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.Automaton.Rare;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Nodes;
using Downfall.Code.Patches;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;

namespace Downfall.Code.Vfx;

[GlobalClass]
public partial class NSequenceDisplay : Control
{
    public const float SequencedCardScale = 0.28f;

    private const string DisplayScenePath = "res://Downfall/scenes/ui/automaton_display.tscn";

    private Control?         _previewContainer;
    private NGridCardHolder? _previewHolder;
    private FunctionCard?    _previewModel;
    private Player?          _trackedPlayer;

    // Pre-placed in scene — positions are set in the .tscn
    private readonly List<NAutomatonSlot>  _slots       = [];
    private readonly List<NGridCardHolder> _cardHolders = [];

    private readonly float[] _bobOffsets = new float[4];
    private readonly float[] _bobSpeeds  = [1.1f, 0.9f, 1.05f, 0.95f];
    private float _bobTime;

    public static NSequenceDisplay Create(Player player)
    {
        var scene = ResourceLoader.Load<PackedScene>(DisplayScenePath);
        var node  = scene.Instantiate<NSequenceDisplay>();
        node._trackedPlayer = player;
        node.Scale = Vector2.One * SequencedCardScale;
        return node;
    }

    public override void _Ready()
    {
        _slots.Add(GetNode<NAutomatonSlot>("%Slot0"));
        _slots.Add(GetNode<NAutomatonSlot>("%Slot1"));
        _slots.Add(GetNode<NAutomatonSlot>("%Slot2"));
        _slots.Add(GetNode<NAutomatonSlot>("%Slot3"));
        _previewContainer = GetNode<Control>("%FuncPreview");
    }

    public Vector2 GetSlotGlobalPosition(int index)
    {
        return index < _slots.Count ? _slots[index].CardAnchorGlobal : GlobalPosition;
    }
public void Refresh()
{
    if (_trackedPlayer == null) return;

    // Clear previous cards
    foreach (var h in _cardHolders) FindOnTablePatch.Unregister(h.CardModel);
    _cardHolders.Clear();

    foreach (var slot in _slots) slot.ClearCard();

    _previewHolder?.QueueFree();
    _previewHolder = null;
    _previewModel  = null;

    if (_previewContainer != null)
        foreach (var child in _previewContainer.GetChildren())
            child.QueueFree();

    var sequence = AutomatonCmd.GetSequence(_trackedPlayer);
    var max      = AutomatonCmd.GetMax(_trackedPlayer);

    // 1. Update Slots: HBox handles layout automatically based on Visibility
    for (var i = 0; i < _slots.Count; i++)
    {
        var slot = _slots[i];
        slot.Visible = i < max;

        if (i >= max || i >= sequence.Count) continue;

        var cardNode = NCard.Create(sequence[i]);
        if (cardNode == null) continue;

        var holder = slot.SetCard(cardNode);
        if (holder == null) { cardNode.QueueFree(); continue; }

        holder.SetClickable(true);
        var captured = i;
        holder.Pressed += _ => NGame.Instance?.GetInspectCardScreen()
            .Open(AllCardsForInspect(), captured);

        cardNode.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
        FindOnTablePatch.Register(sequence[i], cardNode);
        _cardHolders.Add(holder);
    }

    // 2. Function card preview logic
    var snapshot = sequence.OfType<AutomatonCardModel>().ToList();
    
    // Hide preview slot if no cards are encoded
    _previewContainer!.Visible = snapshot.Count > 0;
    if (snapshot.Count == 0) return;

    FunctionCard? previewCanonical;
    if (snapshot.Any(c => c is FullRelease))
        previewCanonical = ModelDb.Card<FunctionPowerCard>();
    else if (snapshot.Any(c => c.Type == CardType.Attack))
        previewCanonical = ModelDb.Card<FunctionAttackCard>();
    else
        previewCanonical = ModelDb.Card<FunctionSkillCard>();

    _previewModel = previewCanonical?.ToMutable() as FunctionCard;
    if (_previewModel == null) return;

    _previewModel.SetSourceCards(snapshot);
    foreach (var card in snapshot) card.ApplyToFunctionPreview(_previewModel);
    _previewModel.Owner = _trackedPlayer;

    var funcCardNode = NCard.Create(_previewModel);
    if (funcCardNode == null) return;

    _previewHolder = NGridCardHolder.Create(funcCardNode);
    if (_previewHolder == null) { funcCardNode.QueueFree(); return; }
    _previewHolder.Scale = Vector2.One * 1.5f;
    // No manual position math needed here anymore! 
    _previewContainer.AddChild(_previewHolder);
    
    // Center the card inside the FuncPreview control area
    _previewHolder.Position = _previewContainer.Size / 2f - _previewHolder.Size / 2f;

    _previewHolder.SetClickable(true);
    _previewHolder.Pressed += _ =>
    {
        var cards = AllCardsForInspect();
        NGame.Instance?.GetInspectCardScreen().Open(cards, cards.Count - 1);
    };

    funcCardNode.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
}

    public override void _Process(double delta)
    {
        if (_trackedPlayer == null || !CombatManager.Instance.IsInProgress) return;

        _bobTime += (float)delta;
        for (var i = 0; i < 4; i++)
            _bobOffsets[i] = Mathf.Sin(_bobTime * _bobSpeeds[i] * Mathf.Pi) * 5f;

        for (var i = 0; i < _slots.Count; i++)
            _slots[i].BobOffset = _bobOffsets[i % 4];
    }

    private List<CardModel> AllCardsForInspect()
    {
        var list = _cardHolders.Select(h => h.CardModel).ToList();
        if (_previewModel != null) list.Add(_previewModel);
        return list;
    }
}


[HarmonyPatch(typeof(NCardHolder), "get_SmallScale")]
public static class FunctionCardScalePatch
{
    public static void Postfix(NCardHolder __instance, ref Vector2 __result)
    {
        if (__instance.CardModel is FunctionCard)
        {
            __result = Vector2.One * 1.5f;
        }
    }
}