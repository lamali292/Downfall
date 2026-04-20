using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.Automaton.Rare;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Commands;
using Downfall.Code.Nodes;
using Downfall.Code.Patches;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Downfall.Code.Vfx;

[GlobalClass]
public partial class NSequenceDisplay : Control
{
    private const float SequencedCardScale = 0.28f;

    private const string DisplayScenePath = "res://Downfall/scenes/ui/automaton_display.tscn";

    private Control? _previewContainer;
    private NCustomCardHolder? _previewHolder;
    private FunctionCard? _previewModel;
    private Player? _trackedPlayer;
    private CombatManager? _combatManager;
    private int _currentMax = 3;

    private readonly List<NAutomatonSlot> _slots = [];
    private readonly List<NCustomCardHolder> _cardHolders = [];
    private List<CardModel> _lastSequence = [];
    private bool _initialized;

    private readonly float[] _bobOffsets = new float[4];
    private readonly float[] _lastBobOffsets = new float[4];
    private readonly float[] _bobSpeeds = [1.1f, 0.9f, 1.05f, 0.95f];
    private float _bobTime;

    public static NSequenceDisplay Create(Player player)
    {
        var scene = ResourceLoader.Load<PackedScene>(DisplayScenePath);
        var node = scene.Instantiate<NSequenceDisplay>();
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
        _combatManager = CombatManager.Instance;
    }

    public Vector2 GetSlotGlobalPosition(int index)
    {
        var clamped = Math.Clamp(index, 0, _currentMax - 1);
        return clamped < _slots.Count ? _slots[clamped].CardAnchorGlobal : GlobalPosition;
    }

    public void Refresh(bool force = false)
    {
        if (_trackedPlayer == null) return;

        var sequence = AutomatonCmd.GetSequence(_trackedPlayer);

        if (!force && _initialized && sequence.SequenceEqual(_lastSequence)) return;
        _lastSequence = sequence.ToList();
        _initialized = true;

        _currentMax = AutomatonCmd.GetMax(_trackedPlayer);

        foreach (var h in _cardHolders.Where(h => h.CardModel != null))
        {
            FindOnTablePatch.Unregister(h.CardModel!);
        }

        _cardHolders.Clear();
        foreach (var slot in _slots) slot.ClearCard();

        _previewHolder?.QueueFree();
        _previewHolder = null;
        _previewModel = null;

        if (_previewContainer != null)
            foreach (var child in _previewContainer.GetChildren())
                child.QueueFree();

        // 1. Sequence slots
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

        var snapshot = sequence.OfType<AutomatonCardModel>().ToList();

        FunctionCard? previewCanonical;
        if (snapshot.Any(c => c is FullRelease))
            previewCanonical = ModelDb.Card<FunctionPowerCard>();
        else if (snapshot.Any(c => c.Type == CardType.Attack))
            previewCanonical = ModelDb.Card<FunctionAttackCard>();
        else
            previewCanonical = ModelDb.Card<FunctionSkillCard>();

        _previewModel = previewCanonical.ToMutable() as FunctionCard;
        if (_previewModel == null) return;

        if (snapshot.Count > 0)
        {
            _previewModel.SetSourceCards(snapshot);
            foreach (var card in snapshot) card.ApplyToFunctionPreview(_previewModel);
        }

        _previewModel.Owner = _trackedPlayer;

        var funcCardNode = NCard.Create(_previewModel);
        if (funcCardNode == null || _previewContainer == null) return;

        _previewHolder = NCustomCardHolder.Create(funcCardNode, 1.5f, 2.5f);
        if (_previewHolder == null)
        {
            funcCardNode.QueueFree();
            return;
        }

        _previewHolder.Scale = Vector2.One * 1.5f;
        _previewContainer.AddChild(_previewHolder);
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
        if (_trackedPlayer == null || _combatManager is not { IsInProgress: true }) return;

        _bobTime += (float)delta;
        for (var i = 0; i < 4; i++)
        {
            var newOffset = Mathf.Sin(_bobTime * _bobSpeeds[i] * Mathf.Pi) * 15f;
            if (!(Mathf.Abs(newOffset - _lastBobOffsets[i]) > 0.05f)) continue;
            _bobOffsets[i] = newOffset;
            _lastBobOffsets[i] = newOffset;
            if (i < _slots.Count)
                _slots[i].BobOffset = newOffset;
        }
    }

    private List<CardModel> AllCardsForInspect()
    {
        var list = _cardHolders.Where(h => h.CardModel != null).Select(h => h.CardModel!).ToList();
        if (_previewModel != null) list.Add(_previewModel);
        return list;
    }
}