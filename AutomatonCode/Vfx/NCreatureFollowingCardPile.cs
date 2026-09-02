using Downfall.DownfallCode.Utils.UI;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Automaton.AutomatonCode.Vfx;

public abstract partial class NCreatureFollowingCardPile : NCustomCombatCardPile
{
    private readonly List<NCard> _cardVisuals = new();
    private readonly List<CardModel> _shownModels = new();
    private double _bobTime;

    private Tween? _cardBumpTween;
    private NCreature? _creatureNode;

    private bool _followActive = true;
    protected virtual Vector2 BaseCardOffset => new(0, -250);
    protected virtual float IdleRightSpacing => 22f;
    protected virtual float HoverStepDeg => 12f;
    protected virtual float HoverRightSpacing => 40f;
    
    protected virtual float BigScale => 0.8f;
    protected virtual float SmallScale => 0.25f;

    protected abstract Vector2 FollowOffset { get; }

    protected override bool SelfPositions => true;

    protected virtual float BobAmplitude => 8f;
    protected virtual float BobSpeed => 2f;
    protected virtual float BobPhase => 0f;

    private Vector2 IconCenter =>
        IsInstanceValid(_icon) ? _icon.Position + _icon.Size * 0.5f : Vector2.Zero;

    private Vector2 BasePos => IconCenter + BaseCardOffset;

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
    
    public override void _ExitTree()
    {
        base._ExitTree();
        _followActive = false;
        if (_pile != null)
        {
            _pile.CardAddFinished -= RefreshCardVisual;
            _pile.CardRemoveFinished -= RefreshCardVisual;
            _pile = null;
        }

        _cardBumpTween?.Kill();
        _cardBumpTween = null;
        _creatureNode = null;
        ClearCardVisuals();
    }

    protected override void OnFocus()
    {
        base.OnFocus();
        AnimateTo(true, 0.12, Tween.EaseType.Out, Tween.TransitionType.Cubic);
    }

    protected override void OnUnfocus()
    {
        base.OnUnfocus();
        AnimateTo(false, 0.5, Tween.EaseType.Out, Tween.TransitionType.Expo);
    }

    private (Vector2 pos, float rot) CardLayout(int index, bool fanned)
    {
        if (index == 0)
            return (BasePos, 0f);
        if (!fanned)
            return (BasePos + new Vector2(index * IdleRightSpacing * 0.0f, 0f), 0f);
        var pos = BasePos + new Vector2(index * HoverRightSpacing * 0.0f, -1.5f * index * HoverRightSpacing);
        var rot = Mathf.DegToRad(index * HoverStepDeg * 0.0f);
        return (pos, rot);
    }

    private void RefreshCardVisual()
    {
        RefreshCount();

        if (!IsInstanceValid(this) || !IsInstanceValid(_icon))
            return;

        var models = GetCards();
        if (models.SequenceEqual(_shownModels)) return;

        ClearCardVisuals();
        _shownModels.AddRange(models);

        for (var i = 0; i < models.Count; i++)
        {
            var node = NCard.Create(models[i]);
            if (node == null) continue;

            node.Scale = Vector2.One * SmallScale;
            var (pos, rot) = CardLayout(i, false);
            node.Position = pos;
            node.Rotation = rot;
            node.PivotOffset = IconCenter - pos;
            _cardVisuals.Add(node);
        }

        for (var i = _cardVisuals.Count - 1; i >= 0; i--)
        {
            var node = _cardVisuals[i];
            if (!IsInstanceValid(node)) continue;
            
            AddChild(node);
            node.UpdateVisuals(Pile, CardPreviewMode.None);
        }

        var fanned = IsFocused;
        for (var i = 0; i < _cardVisuals.Count; i++)
        {
            var card = _cardVisuals[i];
            if (!IsInstanceValid(card)) continue;
            var (pos, rot) = CardLayout(i, fanned);
            card.Position = pos;
            card.Rotation = rot;
            card.PivotOffset = IconCenter - pos;
            card.Scale = fanned ? Vector2.One * BigScale : Vector2.One * SmallScale;
        }
    }

    protected virtual List<CardModel> GetCards()
    {
        return _pile?.Cards.ToList() ?? [];
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => GetCards().SelectMany(e => e.HoverTips);

    private void AnimateTo(bool fanned, double time,
        Tween.EaseType ease, Tween.TransitionType trans)
    {
        if (_cardVisuals.Count == 0) return;

        _cardBumpTween?.Kill();
        _cardBumpTween = CreateTween().SetParallel();

        for (var i = 0; i < _cardVisuals.Count; i++)
        {
            var card = _cardVisuals[i];
            if (!IsInstanceValid(card)) continue;

            var (pos, rot) = CardLayout(i, fanned);
            _cardBumpTween.TweenProperty(card, "position", pos, time).SetEase(ease).SetTrans(trans);
            _cardBumpTween.TweenProperty(card, "rotation", rot, time).SetEase(ease).SetTrans(trans);
            _cardBumpTween.TweenProperty(card, "pivot_offset", IconCenter - pos, time).SetEase(ease).SetTrans(trans);
            var scale2 = fanned ? Vector2.One * BigScale : Vector2.One * SmallScale;
            _cardBumpTween.TweenProperty(card, "scale", scale2, time).SetEase(ease).SetTrans(trans);
        }
    }

    private void ClearCardVisuals()
    {
        foreach (var card in _cardVisuals.Where(IsInstanceValid))
            card.QueueFree();
        _cardVisuals.Clear();
        _shownModels.Clear();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!_followActive) return;
        if (!IsInstanceValid(this)) return;
        if (!Visible) return;
        if (!CombatManager.Instance.IsInProgress) return;
        if (_localPlayer == null) return;

        if (_creatureNode == null || !IsInstanceValid(_creatureNode))
        {
            _creatureNode = NCombatRoom.Instance?.GetCreatureNode(_localPlayer.Creature);
            if (_creatureNode == null || !IsInstanceValid(_creatureNode)) return;
        }

        if (!IsInstanceValid(_creatureNode.Visuals)) return;

        _bobTime += delta;
        var bob = new Vector2(0f, Mathf.Sin((float)_bobTime * BobSpeed + BobPhase) * BobAmplitude);

        GlobalPosition = _creatureNode.GlobalPosition +
                         (new Vector2(-75, -75) + FollowOffset + ButtonOffsets + bob) * _creatureNode.Visuals.Scale;
        Scale = _creatureNode.Visuals.Scale;
    }

    public override void AnimIn()
    {
        if (!IsInstanceValid(this)) return;
        Visible = true;
        _followActive = false;

        var target = CurrentFollowPosition();
        if (target == null)
        {
            _followActive = true;
            return;
        }

        GlobalPosition = target.Value + HideOffset;

        _positionTween?.Kill();
        _positionTween = CreateTween();
        _positionTween.TweenProperty(this, "global_position", target.Value, 0.5)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Expo);

        _positionTween.Finished += () =>
        {
            if (IsInstanceValid(this))
            {
                _followActive = true;
            }
        };
    }

    public override void PlayAnimOut()
    {
        if (!IsInstanceValid(this)) return;
        _followActive = false;
        _positionTween?.Kill();
        _positionTween = CreateTween();

        _positionTween.TweenProperty(this, "global_position", GlobalPosition + HideOffset, 0.5)
            .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Back);
        _positionTween.Finished += () =>
        {
            if (IsInstanceValid(this)) Visible = false;
        };
    }

    private Vector2? CurrentFollowPosition()
    {
        if (_localPlayer == null) return null;
        var c = NCombatRoom.Instance?.GetCreatureNode(_localPlayer.Creature);
        if (c == null || !IsInstanceValid(c) || !IsInstanceValid(c.Visuals)) return null;
        return c.GlobalPosition + (new Vector2(-75, -75) + FollowOffset + ButtonOffsets) * c.Visuals.Scale;
    }
}