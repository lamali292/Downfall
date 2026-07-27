using Downfall.DownfallCode.Nodes;
using Downfall.DownfallCode.Patches;
using Downfall.DownfallCode.Utils.UI;
using Godot;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;

namespace Guardian.GuardianCode.Vfx;

[GlobalClass]
public partial class NGuardianDisplay : Control
{
    private const float SequencedCardScale = 1f;
    private const string DisplayScenePath = "res://Guardian/scenes/guardian_display.tscn";
    private const string StasisSlotScenePath = "res://Guardian/scenes/stasis_slot.tscn";

    private readonly List<NCustomCardHolder> _cardHolders = [];
    private readonly List<NStasisSlot> _slots = [];
    
    private readonly Dictionary<NCustomCardHolder, NCardHolder.PressedEventHandler> _pressedHandlers = [];

    private Control? _creatureHitbox;
    private int _currentMax = 3;
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
        ReleaseAllCards();
    }
    private void ReleaseHolder(NCustomCardHolder holder)
    {
        if (_pressedHandlers.Remove(holder, out var handler)
            && IsInstanceValid(holder))
            holder.Pressed -= handler;

        if (IsInstanceValid(holder) && holder.CardModel != null)
            FindOnTablePatch.Unregister(holder.CardModel);
    }

    private void ReleaseAllCards()
    {
        foreach (var h in _cardHolders)
            ReleaseHolder(h);
        _cardHolders.Clear();
        _pressedHandlers.Clear();
    }
    
    private void EnsureSlotCount(int count)
    {
        if (_slotContainer == null || _stasisSlotScene == null) return;

        while (_slots.Count > count)
        {
            var last = _slots[^1];
            _slots.RemoveAt(_slots.Count - 1);
            last.QueueFree();
        }

        while (_slots.Count < count)
        {
            var slot = _stasisSlotScene.Instantiate<NStasisSlot>();
            _slotContainer.AddChild(slot);
            _slots.Add(slot);
        }
    }

    public void RefreshCounters()
    {
        if (_trackedPlayer == null) return;

        var sequence = _trackedPlayer.GetStasis().ToList();
        for (var i = 0; i < _slots.Count && i < sequence.Count; i++)
            _slots[i].UpdateCounterDisplay(sequence[i]);
    }

    public void Refresh()
    {
        if (_trackedPlayer == null) return;

        var sequence = _trackedPlayer.GetStasis().ToList();
        _currentMax = GuardianCmd.GetMaxStasisSlots(_trackedPlayer);
        
        ReleaseAllCards();
        foreach (var slot in _slots)
            slot.ClearCard();
        EnsureSlotCount(_currentMax);
        for (var i = 0; i < _slots.Count; i++)
        {
            var slot = _slots[i];
            slot.Visible = i < _currentMax;
            if (i >= _currentMax || i >= sequence.Count) continue;

            var model = sequence[i];
            var cardNode = NCard.Create(model);
            if (cardNode == null) continue;

            var holder = slot.SetCard(cardNode);
            if (holder == null)
            {
                cardNode.QueueFree(); 
                continue;
            }

            holder.SetClickable(true);
            WireInspect(holder);

            cardNode.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
            FindOnTablePatch.Register(model, cardNode);
            _cardHolders.Add(holder);
        }

        DownfallControllerNav.WireChain(_cardHolders, true, true);
        if (_creatureHitbox != null)
            DownfallControllerNav.LinkAbove(_cardHolders, _creatureHitbox);

        RefreshCounters();
    }
    
    private void WireInspect(NCustomCardHolder holder)
    {
        NCardHolder.PressedEventHandler handler = _ =>
        {
            if (!IsInstanceValid(holder) || holder.CardModel == null) return;

            var cards = AllCardsForInspect();
            var idx = cards.IndexOf(holder.CardModel);
            if (idx < 0) return;

            NGame.Instance?.GetInspectCardScreen().Open(cards, idx);
        };

        holder.Pressed += handler;
        _pressedHandlers[holder] = handler;
    }

    private List<CardModel> AllCardsForInspect()
    {
        return _cardHolders
            .Where(h => IsInstanceValid(h) && h.CardModel != null)
            .Select(h => h.CardModel!)
            .ToList();
    }

    private Vector2 GetSlotGlobalPosition(int index)
    {
        var clamped = Math.Clamp(index, 0, _currentMax - 1);
        return clamped < _slots.Count
            ? _slots[clamped].CardAnchorGlobal
            : GlobalPosition;
    }

    public NCard? GetNCard(CardModel card)
    {
        var cardNode = _cardHolders.Find(h => IsInstanceValid(h) && h.CardModel == card)?.CardNode;
        if (cardNode != null && IsInstanceValid(cardNode) && cardNode.Model == card)
            return cardNode;

        return null;
    }

    public Vector2? GetTargetPosition(CardModel card)
    {
        if (_trackedPlayer == null) return GlobalPosition;

        var sequence = _trackedPlayer.GetStasis().ToList();

        var existingIndex = sequence.IndexOf(card);
        if (existingIndex >= 0)
            return existingIndex < _slots.Count ? _slots[existingIndex].CardAnchorGlobal : GlobalPosition;

        var nextIndex = sequence.Count;
        if (nextIndex >= _currentMax) nextIndex = _currentMax - 1;

        return nextIndex >= 0 && nextIndex < _slots.Count
            ? _slots[nextIndex].CardAnchorGlobal
            : GlobalPosition;
    }
}