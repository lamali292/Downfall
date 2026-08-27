using System.Collections.Generic;
using System.Linq;
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
    private const float HoverMul = 1.25f;
    private const float IdleRightSpacing = 22f;  // Versatz nach rechts pro Karte (Ruhe)
    private const float HoverStepDeg = 12f;      // Rotationsschritt pro Karte (Hover)
    private const float HoverRightSpacing = 40f; // zusätzlicher Versatz nach rechts beim Hover
    private static readonly Vector2 BaseCardOffset = new(0, -300); // Abstand Karte -> Icon = Radius

    private Tween? _cardBumpTween;
    private readonly List<NCard> _cardVisuals = new();
    private readonly List<CardModel> _shownModels = new();

    protected override PileType Pile => StashPile.Stash;
    public override string ScenePath => "res://Automaton/scenes/ui/stash_pile.tscn";
    protected override Vector2 HideOffset => new(-160f, 100f);
    protected override Vector2 HoverTipOffset => new(30f, -850f);
    protected override Vector2 ButtonOffsets => new(115f, -360f);

    protected override bool StartHidden(Player player) => player.Character is not Core.Automaton;

    protected override HoverTip BuildHoverTip()
    {
        var description = new LocString("static_hover_tips", "AUTOMATON-STASH.description");
        return new HoverTip(
            new LocString("static_hover_tips", "AUTOMATON-STASH.title"),
            description);
    }

    protected override void OnFocus()
    {
        base.OnFocus();
        AnimateTo(fanned: true, CardScale * HoverMul, 0.12,
            Tween.EaseType.Out, Tween.TransitionType.Cubic);
    }

    protected override void OnUnfocus()
    {
        base.OnUnfocus();
        AnimateTo(fanned: false, CardScale, 0.5,
            Tween.EaseType.Out, Tween.TransitionType.Expo);
    }

    protected override LocString BuildEmptyPileMessage()
        => new LocString("combat_messages", "OPEN_EMPTY_STASH");

    public override void Initialize(Player player)
    {
        base.Initialize(player);
        if (_pile != null)
        {
            _pile.CardAddFinished += RefreshCardVisual;
            _pile.CardRemoveFinished += RefreshCardVisual;
        }
        RefreshCardVisual();
    }

    private Vector2 IconCenter => _icon.Position + _icon.Size * 0.5f;
    private Vector2 BasePos => IconCenter + BaseCardOffset;
    
    private (Vector2 pos, float rot) CardLayout(int index, bool fanned)
    {
        if (index == 0)
            return (BasePos, 0f);

        if (!fanned)
            return (BasePos + new Vector2(index * IdleRightSpacing * 0.0f, 0f), 0f);

        var pos = BasePos + new Vector2(index * HoverRightSpacing * 0.0f , -2*index*HoverRightSpacing);
        var rot = Mathf.DegToRad(index * HoverStepDeg * 0.0f);
        return (pos, rot);
    }

    private void RefreshCardVisual()
    {
        var models = _pile?.Cards.ToList() ?? new List<CardModel>();
        if (models.SequenceEqual(_shownModels)) return;

        ClearCardVisuals();
        _shownModels.AddRange(models);
        
        for (int i = 0; i < models.Count; i++)
        {
            var node = NCard.Create(models[i]);
            if (node == null) continue;

            node.Scale = Vector2.One * CardScale;

            var (pos, rot) = CardLayout(i, fanned: false);
            node.Position = pos;
            node.Rotation = rot;
            node.PivotOffset = IconCenter - pos; 

            _cardVisuals.Add(node);
        }
        
        for (int i = _cardVisuals.Count - 1; i >= 0; i--)
        {
            var node = _cardVisuals[i];
            AddChild(node);
            node.UpdateVisuals(StashPile.Stash, CardPreviewMode.None);
        }
    }

    private void AnimateTo(bool fanned, float scale, double time,
        Tween.EaseType ease, Tween.TransitionType trans)
    {
        if (_cardVisuals.Count == 0) return;

        _cardBumpTween?.Kill();
        _cardBumpTween = CreateTween().SetParallel();
        
        for (int i = 0; i < _cardVisuals.Count; i++)
        {
            var card = _cardVisuals[i];
            if (!IsInstanceValid(card)) continue;

            var (pos, rot) = CardLayout(i, fanned);
            _cardBumpTween.TweenProperty(card, "position", pos, time).SetEase(ease).SetTrans(trans);
            _cardBumpTween.TweenProperty(card, "rotation", rot, time).SetEase(ease).SetTrans(trans);
            _cardBumpTween.TweenProperty(card, "pivot_offset", IconCenter - pos, time).SetEase(ease).SetTrans(trans);
            var scale2 = fanned ? (Vector2.One * scale) : (Vector2.One * scale * 0.5f);
            _cardBumpTween.TweenProperty(card, "scale", scale2, time).SetEase(ease).SetTrans(trans);
        }
    }

    private void ClearCardVisuals()
    {
        foreach (var card in _cardVisuals)
            if (card != null && IsInstanceValid(card))
                card.QueueFree();
        _cardVisuals.Clear();
        _shownModels.Clear();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (_pile != null)
        {
            _pile.CardAddFinished -= RefreshCardVisual;
            _pile.CardRemoveFinished -= RefreshCardVisual;
        }
        _cardBumpTween?.Kill();
        _cardBumpTween = null;
        ClearCardVisuals();
    }
}