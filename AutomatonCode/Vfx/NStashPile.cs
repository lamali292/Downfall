using Automaton.AutomatonCode.Piles;
using Downfall.DownfallCode.Utils.UI;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Automaton.AutomatonCode.Vfx;

public partial class NStashPile : NCustomCombatCardPile
{
    private const float CardScale = 0.4f;
    private const float HoverMul = 1.25f; // matches the base icon hover scale
    private Tween? _cardBumpTween;

    private NCard? _cardVisual;
    private CardModel? _shownModel;

    protected override PileType Pile => StashPile.Stash;
    public override string ScenePath => "res://Automaton/scenes/ui/stash_pile.tscn";
    protected override Vector2 HideOffset => new(-160f, 100f);
    protected override Vector2 HoverTipOffset => new(30f, -850f);
    protected override Vector2 ButtonOffsets => new(115f, -360f);

    protected override HoverTip BuildHoverTip()
    {
        var description = new LocString("static_hover_tips", "AUTOMATON-STASH.description");
        return new HoverTip(
            new LocString("static_hover_tips", "AUTOMATON-STASH.title"),
            description);
    }

    protected override void OnFocus()
    {
        base.OnFocus(); // hover tip + icon bump (its own private tween)
        if (_cardVisual == null || !IsInstanceValid(_cardVisual)) return;

        _cardBumpTween?.Kill();
        _cardBumpTween = CreateTween();
        _cardBumpTween.TweenProperty(_cardVisual, "scale",
            Vector2.One * (CardScale * HoverMul), 0.05);
    }

    protected override void OnUnfocus()
    {
        base.OnUnfocus();
        if (_cardVisual == null || !IsInstanceValid(_cardVisual)) return;

        _cardBumpTween?.Kill();
        _cardBumpTween = CreateTween();
        _cardBumpTween.TweenProperty(_cardVisual, "scale", Vector2.One * CardScale, 0.5)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
    }

    protected override LocString BuildEmptyPileMessage()
    {
        return new LocString("combat_messages", "OPEN_EMPTY_STASH");
    }

    // --- Card visual ---

    public override void Initialize(Player player)
    {
        base.Initialize(player); // subscribes base AddCard/RemoveCard to CardAddFinished/RemoveFinished (count + plop)

        // Show the card on the LANDING, not the logical add: CardAddFinished fires when the
        // fly animation reaches the pile (same event that drives the base plop), whereas
        // CardAdded fires immediately on the logical add — which is why the card popped in early.
        if (_pile != null)
        {
            _pile.CardAddFinished += RefreshCardVisual;
            _pile.CardRemoveFinished += RefreshCardVisual;
        }

        RefreshCardVisual(); // pre-existing stash cards (combat start / reconnect): no animation to wait for
    }

    private void RefreshCardVisual()
    {
        var next = _pile?.Cards.FirstOrDefault(); // next-draw = pile front (as the old preview used)
        if (next == _shownModel) return; // front unchanged (e.g. add went to the bottom) -> keep node

        ClearCardVisual();
        _shownModel = next;
        if (next == null) return;

        var node = NCard.Create(next);
        if (node == null)
        {
            _shownModel = null;
            return;
        }

        _cardVisual = node;

        node.Scale = Vector2.One * CardScale;

        AddChild(node);
        node.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);

        var iconCenter = _icon.Position + _icon.Size * 0.5f;
        var cardOffset = new Vector2(0, -300);
        node.Position = iconCenter + cardOffset;

        node.PivotOffset = iconCenter - node.Position;
    }

    private void ClearCardVisual()
    {
        if (_cardVisual != null && IsInstanceValid(_cardVisual))
            _cardVisual.QueueFree();
        _cardVisual = null;
        _shownModel = null;
    }

    public override void _ExitTree()
    {
        base._ExitTree(); // base unsubscribes its own AddCard/RemoveCard + kills tweens

        if (_pile != null)
        {
            _pile.CardAddFinished -= RefreshCardVisual;
            _pile.CardRemoveFinished -= RefreshCardVisual;
        }

        _cardBumpTween?.Kill();
        _cardBumpTween = null;
        ClearCardVisual();
    }
}