using Downfall.DownfallCode.Nodes;
using Downfall.DownfallCode.Patches;
using Downfall.DownfallCode.Utils.UI;
using Godot;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Guardian.GuardianCode.Vfx;

[GlobalClass]
public partial class NGuardianDisplay : Control
{
    private const float SequencedCardScale = 1;
    private const string DisplayScenePath = "res://Guardian/scenes/guardian_display.tscn";
    private const string StasisSlotScenePath = "res://Guardian/scenes/stasis_slot.tscn";
    private readonly List<NCustomCardHolder> _cardHolders = [];

    private readonly List<NStasisSlot> _slots = [];
    private float _bobTime;
    private Control? _creatureHitbox;
    private int _currentMax = 3;
    private bool _initialized;

    private HBoxContainer? _slotContainer;
    private PackedScene? _stasisSlotScene;

    private Player? _trackedPlayer;

    public static NGuardianDisplay Create(Player player, Control? creatureHitbox)
    {
        var scene = ResourceLoader.Load<PackedScene>(DisplayScenePath);
        var node = scene.Instantiate<NGuardianDisplay>();
        node._trackedPlayer = player;
        node._creatureHitbox = creatureHitbox;
        node.Scale = Vector2.One * SequencedCardScale;
        return node;
    }

    public override void _Ready()
    {
        _slotContainer = GetNode<HBoxContainer>("%SlotContainer");
        _stasisSlotScene = ResourceLoader.Load<PackedScene>(StasisSlotScenePath);
    }

    public override void _ExitTree()
    {
        // Combat end / room teardown: unregister everything we put into the
        // FindOnTable registry so it can never serve this display's dead nodes
        // in a later combat (CardModels persist across fights).
        ReleaseAllCards();
    }

    /// <summary>
    ///     Display card nodes can be ADOPTED by the base game: when a stasis card
    ///     moves to another pile, NCard.FindOnTable (via FindOnTablePatch) hands the
    ///     engine our node and the engine reparents it into the hand/play flow.
    ///     From that moment the node is no longer ours.
    ///
    ///     So on cleanup we only destroy a node that is still under this display
    ///     (IsAncestorOf). If it was reparented away, we just drop our references
    ///     and let the game manage its lifecycle (it will pool-free it itself).
    /// </summary>
    private void ReleaseHolder(NCustomCardHolder holder)
    {
        if (holder.CardModel != null)
            FindOnTablePatch.Unregister(holder.CardModel);

        var cardNode = holder.CardNode;
        if (cardNode == null || !IsInstanceValid(cardNode)) return;

        var stillOwned = cardNode.IsInsideTree() && IsAncestorOf(cardNode);
        if (!stillOwned) return; // adopted by the hand/play flow — hands off

        cardNode.GetParent()?.RemoveChild(cardNode);
        cardNode.QueueFree();
    }

    private void ReleaseAllCards()
    {
        foreach (var h in _cardHolders) ReleaseHolder(h);
        _cardHolders.Clear();
    }

    private void EnsureSlotCount(int count)
    {
        if (_slotContainer == null || _stasisSlotScene == null) return;
        while (_slots.Count > count)
        {
            var lastSlot = _slots[^1];
            _slots.RemoveAt(_slots.Count - 1);
            lastSlot.QueueFree(); // safe: runs after ReleaseAllCards, slots are empty
        }

        while (_slots.Count < count)
        {
            var slot = _stasisSlotScene.Instantiate<NStasisSlot>();
            _slotContainer.AddChild(slot);
            _slots.Add(slot);
        }
    }

    public Vector2 GetSlotGlobalPosition(int index)
    {
        var clamped = Math.Clamp(index, 0, _currentMax - 1);
        return clamped < _slots.Count ? _slots[clamped].CardAnchorGlobal : GlobalPosition;
    }

    public void RefreshCounters()
    {
        if (_trackedPlayer == null) return;

        var sequence = GuardianCmd.GetStasisCards(_trackedPlayer);

        for (var i = 0; i < _slots.Count && i < sequence.Count; i++)
            _slots[i].UpdateCounterDisplay(sequence[i]);
    }

    public void Refresh()
    {
        if (_trackedPlayer == null) return;

        var sequence = GuardianCmd.GetStasisCards(_trackedPlayer);
        _currentMax = GuardianCmd.GetMaxStasisSlots(_trackedPlayer);
        _initialized = true;

        // Order matters:
        // 1) unregister + destroy only the nodes we still own
        ReleaseAllCards();
        // 2) clear the (now empty) holders
        foreach (var slot in _slots) slot.ClearCard();
        // 3) only now shrink/grow — shrinking earlier could QueueFree a slot
        //    that still contained a live card
        EnsureSlotCount(_currentMax);

        for (var i = 0; i < _slots.Count; i++)
        {
            var slot = _slots[i];
            slot.Visible = i < _currentMax;

            if (i >= _currentMax || i >= sequence.Count) continue;

            var cardNode = NCard.Create(sequence[i]);
            if (cardNode == null) continue;

            var holder = slot.SetCard(cardNode);
            if (holder == null)
            {
                // fresh node, nothing else references it yet — safe to discard
                cardNode.QueueFree();
                continue;
            }

            holder.SetClickable(true);
            var captured = i;
            holder.Pressed += _ => NGame.Instance?.GetInspectCardScreen()
                .Open(AllCardsForInspect(), captured);

            cardNode.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
            FindOnTablePatch.Register(sequence[i], cardNode);
            _cardHolders.Add(holder);
        }

        DownfallControllerNav.WireChain(_cardHolders, wrap: true, rtl: true);
        if (_creatureHitbox != null)
            DownfallControllerNav.LinkAbove(_cardHolders, _creatureHitbox);

        RefreshCounters();
    }

    private List<CardModel> AllCardsForInspect()
    {
        return _cardHolders.Where(h => h.CardModel != null).Select(h => h.CardModel!).ToList();
    }

    public NCard? GetNCard(CardModel card)
    {
        var cardNode = _cardHolders.Find(h => h.CardModel == card)?.CardNode;

        // Also verify the model still matches: a pooled node can be alive but
        // recycled to display a different card.
        if (cardNode != null && IsInstanceValid(cardNode) && cardNode.Model == card)
            return cardNode;

        return null;
    }

    public Vector2? GetTargetPosition(CardModel card)
    {
        if (_trackedPlayer == null) return GlobalPosition;

        var sequence = GuardianCmd.GetStasisCards(_trackedPlayer);
        var existingIndex = sequence.IndexOf(card);
        if (existingIndex >= 0)
            return existingIndex < _slots.Count ? _slots[existingIndex].CardAnchorGlobal : GlobalPosition;
        var nextIndex = sequence.Count;
        if (nextIndex >= _currentMax)
            nextIndex = _currentMax - 1;

        return nextIndex < _slots.Count ? _slots[nextIndex].CardAnchorGlobal : GlobalPosition;
    }
}