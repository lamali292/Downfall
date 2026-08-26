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
    private Tween? _ownBumpTween;
    private Tween? _revealTween;

    private Vector2 _cachedShow;
    private bool _hasCachedShow;

    protected abstract override PileType Pile { get; }
    public abstract string ScenePath { get; }
    protected abstract Vector2 HideOffset { get; }
    protected abstract Vector2 HoverTipOffset { get; }
    protected abstract Vector2 ButtonOffsets { get; }
    protected abstract HoverTip BuildHoverTip();
    protected abstract LocString BuildEmptyPileMessage();

    protected virtual IEnumerable<IHoverTip> ExtraHoverTips => [];
    protected virtual bool StartHidden(Player player) => false;

    public override void _Ready()
    {
        ConnectSignals();                 // base populates _icon and _countLabel here
        _emptyPileMessage = BuildEmptyPileMessage();

        var size = Size;
        OffsetLeft   = ButtonOffsets.X;
        OffsetTop    = ButtonOffsets.Y;
        OffsetRight  = ButtonOffsets.X + size.X;
        OffsetBottom = ButtonOffsets.Y + size.Y;

        _cachedShow = Position;
        _hasCachedShow = true;
        ApplyAnimPositions();
    }

    public static Vector2 GetPositionFor<T>()  where T : NCustomCombatCardPile
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
        var show = _hasCachedShow ? _cachedShow : Position;
        _showPosition = show;
        _hidePosition = show + HideOffset;
    }

    protected override void SetAnimInOutPositions() => ApplyAnimPositions();
    public void RefreshAnimPositions() => ApplyAnimPositions();

    public override void Initialize(Player player)
    {
        base.Initialize(player);          // sets _localPlayer, _pile, _currentCount, label, base handlers
        if (StartHidden(player)) Visible = false;
    }

    /// Resync the count label to the true pile size. Call after AddInternal/RemoveInternal,
    /// which bypass the CardAddFinished/CardRemoveFinished events the base listens to.
    public void RefreshCount()
    {
        if (_pile == null) return;
        _currentCount = _pile.Cards.Count;
        _countLabel.SetTextAutoSize(_currentCount.ToString());
    }

    public void Reveal()
    {
        RefreshCount(); 
        if (Visible) return;
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

internal static class CombatPileButtonRegistry
{
    private static List<Type>? _types;

    internal static IReadOnlyList<Type> Types => _types ??= Discover();

    private static List<Type> Discover()
    {
        var results = new List<Type>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            IEnumerable<Type> types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null)!;
            }

            results.AddRange(types.Where(t =>
                t is { IsClass: true, IsAbstract: false } &&
                t.IsSubclassOf(typeof(NCustomCombatCardPile))));
        }

        return results;
    }

    internal static string ReadMetadata(Type type)
    {
        var probe = (NCustomCombatCardPile)RuntimeHelpers.GetUninitializedObject(type);
        return probe.ScenePath;
    }
}