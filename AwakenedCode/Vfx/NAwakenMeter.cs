using Awakened.AwakenedCode.CustomEnums;
using Champ.ChampCode.Vfx;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;

namespace Awakened.AwakenedCode.Vfx;

public partial class NAwakenMeter : Control
{
    private const string DisplayScenePath = "res://Awakened/scenes/ui/awaken_meter.tscn";
    public bool IsExiting => _isExiting;
    private NinePatchRect? _bar;
    private const int MaxProgress = 7;
    private Func<IEnumerable<IHoverTip>>? _tipProvider;
    private IEnumerable<IHoverTip>? _tips;
    private Vector2 _restPosition;
    private Vector2 RelativeOffset => new Vector2(-80f, -100f);
    private Vector2 HideOffset => new Vector2(-120f, 0f);

    private Tween? _sizeTween;
    private Tween? _moveTween;
    private bool _isExiting;

    public override void _EnterTree()
    {
        base._EnterTree();
        CombatManager.Instance.CombatEnded += OnCombatEnded;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        CombatManager.Instance.CombatEnded -= OnCombatEnded;
    }

    private void OnCombatEnded(CombatRoom room) => AnimOutAndFree();

    public override void _Ready()
    {
        _bar = GetNode<NinePatchRect>("%AwakenForeground");
        AnchorTop = 1f;
        AnchorBottom = 1f;
        MouseFilter = MouseFilterEnum.Stop;
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
        Modulate = new Color(Modulate, 0f);
        
        var timer = GetTree().CreateTimer(0.7);
        timer.Timeout += () =>
        {
            if (!IsInstanceValid(this)) return;
            _restPosition = GetTargetShowPosition();
            AnimIn();
        };
        SetTipProvider(() => [HoverTipFactory.Static(AwakenedTip.Awaken)]);
    }

    
    public void SetTipProvider(Func<IEnumerable<IHoverTip>> provider) => _tipProvider = provider;

    private void OnMouseEntered()
    {
        if (_isExiting) return;
        _tips = _tipProvider?.Invoke();
        if (_tips == null) return;

        var tipSet = NHoverTipSet.CreateAndShow(this, _tips);
        if (tipSet == null) return;

        float h = tipSet.TextHoverTipDimensions.Y;
        tipSet.GlobalPosition = GlobalPosition + new Vector2(70f, -h - 100f);
    }

    private void OnMouseExited()
    {
        NHoverTipSet.Remove(this);
    }
    
    private Vector2 GetTargetShowPosition()
    {
        var ui = NCombatRoom.Instance?.Ui;
        var energyNode = ui?._energyCounter;

        GD.Print($"[Awaken] ui={ui != null} energy={energyNode != null} " +
                 $"energyGlobal={energyNode?.GlobalPosition} uiGlobal={ui?.GlobalPosition}");
        if (energyNode == null || ui == null) return Position;
        var uiLocalPos = energyNode.GlobalPosition - ui.GlobalPosition;
        return uiLocalPos + RelativeOffset;

    }

    public static NAwakenMeter? Create(Player player)
    {
        var combatRoom = NCombatRoom.Instance;
        if (combatRoom?.Ui == null)
            return null;

        var scene = ResourceLoader.Load<PackedScene>(DisplayScenePath);
        if (scene == null)
        {
            GD.PrintErr($"[Champ] Could not load {DisplayScenePath}");
            return null;
        }

        var display = scene.Instantiate<NAwakenMeter>();
        display.Scale = new Vector2(3f, 3f);
        combatRoom.Ui.AddChildSafely(display);
        return display;
    }

    private void AnimIn()
    {
        if (_isExiting) return;

        _moveTween?.Kill();

        Position = _restPosition + HideOffset;
        Modulate = new Color(Modulate, 0f);

        _moveTween = CreateTween().SetParallel();
        _moveTween.TweenProperty(this, "position", _restPosition, 0.5f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Expo);
        _moveTween.TweenProperty(this, "modulate:a", 1f, 0.5f)
            .SetEase(Tween.EaseType.Out);
    }

    private void AnimOutAndFree()
    {
        if (_isExiting) return;
        _isExiting = true;

        _moveTween?.Kill();
        _sizeTween?.Kill();

        var targetPos = Position + HideOffset;

        _moveTween = CreateTween().SetParallel();
        _moveTween.TweenProperty(this, "position", targetPos, 0.4f)
            .SetEase(Tween.EaseType.In)
            .SetTrans(Tween.TransitionType.Back);
        _moveTween.TweenProperty(this, "modulate:a", 0f, 0.4f)
            .SetEase(Tween.EaseType.In);

        _moveTween.Finished += OnExitAnimFinished;
    }

    private void OnExitAnimFinished()
    {
        if (IsInstanceValid(this) && !IsQueuedForDeletion())
            QueueFree();
    }

    public void SetProgress(int progress)
    {
        if (_bar == null || _isExiting)
            return;

        progress = Mathf.Clamp(progress, 0, MaxProgress);
        var ratio = (float)progress / MaxProgress;
        var maxWidth = _bar.GetParent<Control>().Size.X;
        var targetSize = new Vector2(21 + maxWidth * ratio, _bar.Size.Y);

        _sizeTween?.Kill();

        _sizeTween = CreateTween();
        _sizeTween.TweenProperty(_bar, "size", targetSize, 0.25f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
    }

    public void Refresh(int value)
    {
        if (!IsInstanceValid(this) || IsQueuedForDeletion() || _isExiting)
            return;
        SetProgress(value);
    }
}