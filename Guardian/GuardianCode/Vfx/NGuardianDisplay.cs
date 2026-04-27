using Downfall.DownfallCode.Nodes;
using Downfall.DownfallCode.Patches;
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
    private const string DisplayScenePath = "res://Downfall/scenes/ui/guardian_display.tscn";
    private const string StasisSlotScenePath = "res://Downfall/scenes/ui/stasis_slot.tscn";
    private readonly List<NCustomCardHolder> _cardHolders = [];

    private readonly List<NStasisSlot> _slots = [];
    private float _bobTime;
    private int _currentMax = 3;
    private bool _initialized;

    private HBoxContainer? _slotContainer;
    private PackedScene? _stasisSlotScene;

    private Player? _trackedPlayer;

    public static NGuardianDisplay Create(Player player)
    {
        var scene = ResourceLoader.Load<PackedScene>(DisplayScenePath);
        var node = scene.Instantiate<NGuardianDisplay>();
        node._trackedPlayer = player;
        node.Scale = Vector2.One * SequencedCardScale;
        return node;
    }

    public override void _Ready()
    {
        _slotContainer = GetNode<HBoxContainer>("%SlotContainer");
        _stasisSlotScene = ResourceLoader.Load<PackedScene>(StasisSlotScenePath);
    }

    private void EnsureSlotCount(int count)
    {
        if (_slotContainer == null || _stasisSlotScene == null) return;
        while (_slots.Count > count)
        {
            var lastSlot = _slots[^1];
            _slots.RemoveAt(_slots.Count - 1);
            lastSlot.QueueFree();
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

        for (var i = 0; i < _slots.Count && i < sequence.Count; i++) _slots[i].UpdateCounterDisplay(sequence[i]);
    }


    public void Refresh()
    {
        if (_trackedPlayer == null) return;

        var sequence = GuardianCmd.GetStasisCards(_trackedPlayer);
        _currentMax = GuardianCmd.GetMaxStasisSlots(_trackedPlayer);
        RefreshCounters();
        _initialized = true;

        EnsureSlotCount(_currentMax);

        foreach (var h in _cardHolders.Where(h => h.CardModel != null)) FindOnTablePatch.Unregister(h.CardModel!);

        _cardHolders.Clear();
        foreach (var slot in _slots) slot.ClearCard();

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

        RefreshCounters();
    }

    private List<CardModel> AllCardsForInspect()
    {
        var list = _cardHolders.Where(h => h.CardModel != null).Select(h => h.CardModel!).ToList();
        return list;
    }


    public NCard? GetNCard(CardModel card)
    {
        var cardNode = _cardHolders.Find(h => h.CardModel == card)?.CardNode;

        if (cardNode != null && IsInstanceValid(cardNode))
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