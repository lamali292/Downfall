using Downfall.Code.Cards.Automaton.Rare;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Displays;
using Downfall.Code.Patches;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;

namespace Downfall.Code.Cards.Vfx;

public partial class NSequenceDisplay : Control
{
    public const float SequencedCardScale = 0.35f;
    private const float FuncCardScale = 0.55f;

    private const float CardDistance = 280 * SequencedCardScale;
    private const float FuncPositionsX = 2.5f * CardDistance + 280 * FuncCardScale / 2f;

    private const string SlotImagePath = "res://Downfall/images/ui/sequenceSlot.png";

    private static readonly Vector2 SlotHalf = new Vector2(76f, 230f) / 2f;

    private readonly float[] _bobOffsets = new float[4];
    private readonly float[] _bobSpeeds = [1.1f, 0.9f, 1.05f, 0.95f];

    private readonly List<Control> _cardContainers = [];
    private readonly List<NGridCardHolder> _cardHolders = [];
    private readonly List<TextureRect> _slotNodes = [];
    private float _bobTime;
    private Control? _previewContainer;
    private NGridCardHolder? _previewHolder;
    private FunctionCard? _previewModel;

    private Player? _trackedPlayer;

    public static NSequenceDisplay Create(Player creature)
    {
        return new NSequenceDisplay
        {
            _trackedPlayer = creature,
            Position = Vector2.Zero
        };
    }

    public Vector2 GetSlotGlobalPosition(int index)
    {
        if (index < _cardContainers.Count)
            return _cardContainers[index].GlobalPosition;
        return GlobalPosition + new Vector2(index * CardDistance, 0f);
    }

    public void Refresh()
    {
        if (_trackedPlayer == null) return;

        foreach (var h in _cardHolders) FindOnTablePatch.Unregister(h.CardModel);
        foreach (var c in _cardContainers) c.QueueFree();
        _cardContainers.Clear();
        _cardHolders.Clear();
        foreach (var s in _slotNodes) s.QueueFree();
        _slotNodes.Clear();
        _previewContainer?.QueueFree();
        _previewContainer = null;
        _previewHolder = null;
        _previewModel = null;

        var sequence = AutomatonCmd.GetSequence(_trackedPlayer);
        var max = AutomatonCmd.GetMax(_trackedPlayer);

        for (var i = 0; i < max; i++)
        {
            // Slot icon
            if (ResourceLoader.Exists(SlotImagePath))
            {
                var slot = new TextureRect
                {
                    Texture = ResourceLoader.Load<Texture2D>(SlotImagePath),
                    StretchMode = TextureRect.StretchModeEnum.KeepAspect,
                    PivotOffset = SlotHalf,
                    Scale = Vector2.One * SequencedCardScale * 3.6f,
                    Position = new Vector2(i * CardDistance, 0f) - SlotHalf
                };
                AddChild(slot);
                _slotNodes.Add(slot);
            }

            // Container scales down — holder inside stays at its natural SmallScale
            var container = new Control
            {
                Scale = Vector2.One * SequencedCardScale,
                Position = new Vector2(i * CardDistance, 0f)
            };
            AddChild(container);
            _cardContainers.Add(container);

            if (i >= sequence.Count) continue;

            var cardNode = NCard.Create(sequence[i]);
            if (cardNode == null) continue;

            var holder = NGridCardHolder.Create(cardNode);
            if (holder == null)
            {
                cardNode.QueueFree();
                continue;
            }

            holder.SetClickable(true);
            var captured = i;
            holder.Pressed += _ => NGame.Instance?.GetInspectCardScreen().Open(AllCardsForInspect(), captured);

            container.AddChild(holder);
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

        _previewModel = previewCanonical?.ToMutable() as FunctionCard;
        if (_previewModel == null) return;

        _previewModel.SetSourceCards(sequence.OfType<AutomatonCardModel>().ToList());
        foreach (var card in sequence.OfType<AutomatonCardModel>()) card.ApplyToFunctionPreview(_previewModel);
        if (_trackedPlayer != null) _previewModel.Owner = _trackedPlayer;

        var funcCardNode = NCard.Create(_previewModel);
        if (funcCardNode == null) return;

        _previewHolder = NGridCardHolder.Create(funcCardNode);
        if (_previewHolder == null)
        {
            funcCardNode.QueueFree();
            return;
        }

        var funcX = FuncPositionsX + (max >= 4 ? CardDistance : 0f);
        _previewContainer = new Control
        {
            Scale = Vector2.One * FuncCardScale,
            Position = new Vector2(funcX, 0f)
        };
        AddChild(_previewContainer);

        _previewHolder.SetClickable(true);
        _previewHolder.Pressed += _ =>
        {
            var cards = AllCardsForInspect();
            NGame.Instance?.GetInspectCardScreen().Open(cards, cards.Count - 1);
        };
        _previewContainer.AddChild(_previewHolder);
        funcCardNode.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
    }

    public override void _Process(double delta)
    {
        if (_trackedPlayer == null || !CombatManager.Instance.IsInProgress) return;

        _bobTime += (float)delta;
        for (var i = 0; i < 4; i++)
            _bobOffsets[i] = Mathf.Sin(_bobTime * _bobSpeeds[i] * Mathf.Pi) * 5f;

        for (var i = 0; i < _cardContainers.Count; i++)
            _cardContainers[i].Position = new Vector2(i * CardDistance, _bobOffsets[i]);

        for (var i = 0; i < _slotNodes.Count; i++)
            _slotNodes[i].Position = new Vector2(i * CardDistance, _bobOffsets[i]) - SlotHalf;

        if (_previewContainer == null) return;
        var funcX = FuncPositionsX + (AutomatonCmd.GetMax(_trackedPlayer) >= 4 ? CardDistance : 0f);
        _previewContainer.Position = new Vector2(funcX, 0f);
    }

    private List<CardModel> AllCardsForInspect()
    {
        var list = _cardHolders.Select(h => h.CardModel).ToList();
        if (_previewModel != null) list.Add(_previewModel);
        return list;
    }
}