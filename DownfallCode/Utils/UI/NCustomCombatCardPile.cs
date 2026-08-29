using System.Reflection;
using System.Runtime.CompilerServices;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Downfall.DownfallCode.Utils.UI;

public abstract partial class NCustomCombatCardPile : NCombatCardPile
{
    private Vector2 _cachedShow;
    private bool _hasCachedShow;
    private Tween? _ownBumpTween;
    private Tween? _revealTween;

    protected abstract override PileType Pile { get; }
    public abstract string ScenePath { get; }
    protected abstract Vector2 HideOffset { get; }
    protected abstract Vector2 HoverTipOffset { get; }
    protected abstract Vector2 ButtonOffsets { get; }

    protected virtual IEnumerable<IHoverTip> ExtraHoverTips => [];
    protected abstract HoverTip BuildHoverTip();
    protected abstract LocString BuildEmptyPileMessage();

    protected virtual bool StartHidden(Player player) => false;
    protected virtual bool StartParkedOffScreen(Player player) => false;
    protected virtual void AfterInitialize(Player player) { }
    protected virtual bool SelfPositions => false;

    public override void _Ready()
    {
        ConnectSignals();
        _emptyPileMessage = BuildEmptyPileMessage();

        var size = Size;
        OffsetLeft = ButtonOffsets.X;
        OffsetTop = ButtonOffsets.Y;
        OffsetRight = ButtonOffsets.X + size.X;
        OffsetBottom = ButtonOffsets.Y + size.Y;

        if (!SelfPositions)
        {
            _cachedShow = Position;
            _hasCachedShow = true;
            ApplyAnimPositions();
        }
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        // Disconnect base AddCard/RemoveCard listeners registered by base._EnterTree()
        if (_pile != null)
        {
            _pile.CardAddFinished -= AddCard;
            _pile.CardRemoveFinished -= RemoveCard;
        }
    }

    public override void Initialize(Player player)
    {
        base.Initialize(player);

        // Disconnect base AddCard/RemoveCard listeners registered by base.Initialize()
        if (_pile != null)
        {
            _pile.CardAddFinished -= AddCard;
            _pile.CardRemoveFinished -= RemoveCard;
        }

        var hidden = StartHidden(player);
        Visible = !hidden;

        if (!hidden && !SelfPositions)
        {
            ApplyAnimPositions();
            Position = StartParkedOffScreen(player) ? _hidePosition : _showPosition;
        }

        AfterInitialize(player);
    }

    public static Vector2 GetPositionFor<T>() where T : NCustomCombatCardPile
    {
        var btn = GetPileNode<T>();
        return btn != null ? btn.GlobalPosition + btn.Size * 0.5f : Vector2.Zero;
    }

    protected static T? GetPileNode<T>() where T : NCustomCombatCardPile
    {
        var container = NCombatRoom.Instance?.Ui._combatPilesContainer;
        if (container == null || !IsInstanceValid(container)) return null;
        return container.GetChildren()
            .OfType<T>()
            .FirstOrDefault(IsInstanceValid);
    }

    private void ApplyAnimPositions()
    {
        if (SelfPositions) return;

        var show = _hasCachedShow ? _cachedShow : Position;
        _showPosition = show;
        _hidePosition = show + HideOffset;
    }

    public virtual void PlayAnimOut() { AnimOut(); }

    protected override void SetAnimInOutPositions()
    {
        ApplyAnimPositions();
    }

    public void RefreshAnimPositions()
    {
        ApplyAnimPositions();
    }

    public void RefreshCount()
    {
        if (_pile == null || !IsInstanceValid(_countLabel)) return;
        _currentCount = _pile.Cards.Count;
        _countLabel.SetTextAutoSize(_currentCount.ToString());
    }

    protected void Reveal()
    {
        RefreshCount();
        if (SelfPositions)
        {
            AnimIn();
            return;
        }   
        if (Visible && Position == _showPosition) return;
        Visible = true;

        ApplyAnimPositions();
        Position = _hidePosition;

        _revealTween?.Kill();
        _revealTween = CreateTween();
        _revealTween.SetPauseMode(Tween.TweenPauseMode.Process);
        _revealTween.TweenProperty(this, "position", _showPosition, 0.4)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Expo);
    }

    protected override void OnFocus()
    {
        NHoverTipSet.Remove(this);
        NHoverTipSet.CreateAndShow(this, [BuildHoverTip(), ..ExtraHoverTips], HoverTipAlignment.Right);

        _ownBumpTween?.Kill();
        _ownBumpTween = CreateTween();
        _ownBumpTween.TweenProperty(_icon, "scale", Vector2.One * 1.25f, 0.05);
    }

    protected override void OnUnfocus()
    {
        NHoverTipSet.Remove(this);

        _ownBumpTween?.Kill();
        _ownBumpTween = CreateTween().SetParallel();
        _ownBumpTween.TweenProperty(_icon, "scale", Vector2.One, 0.5)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
        _ownBumpTween.TweenProperty(_icon, "modulate", Colors.White, 0.5)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
    }
}