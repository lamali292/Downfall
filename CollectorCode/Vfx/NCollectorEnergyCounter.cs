using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Patches;
using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace Collector.CollectorCode.Vfx;

[GlobalClass]
public partial class NCollectorEnergyCounter : Control, IAnimatedCounter
{
    private Tween? _fadeTween;
    private MegaLabel? _label;
    private Player? _player;
    private CollectorEnergy? _resource;
    private Control? _rotationLayers;
    private HoverTip? _hoverTip;

    private Tween? _animInTween;
    private Tween? _animOutTween;
    private Vector2 _showPosition;
    private Vector2 _hidePosition;

    public void Initialize(Player player)
    {
        _player = player;
        _resource = CardResourceRegistry.Get<CollectorEnergy>();
        if (_resource != null)
            _resource.Changed += OnEnergyChanged;
    }

    public override void _Ready()
    {
        _rotationLayers = GetNode<Control>("%RotationLayers");
        _label = GetNode<MegaLabel>("%Label");
        Visible = false;
        Refresh();

        _showPosition = Position;
        _hidePosition = _showPosition + new Vector2(-480f, 128f);
        
        _hoverTip = CollectorTip.ReserveTip;

        Connect(Control.SignalName.MouseEntered, Callable.From(OnHovered));
        Connect(Control.SignalName.MouseExited, Callable.From(OnUnhovered));
    }

    public override void _ExitTree()
    {
        if (_resource != null)
            _resource.Changed -= OnEnergyChanged;
    }

    public override void _Process(double delta)
    {
        if (_rotationLayers == null) return;
        for (var i = 0; i < _rotationLayers.GetChildCount(); i++)
            _rotationLayers.GetChild<Control>(i).RotationDegrees += (float)delta * 30f * (i + 1);
    }

    private void OnEnergyChanged(PlayerCombatState player, int amount)
    {
        if (player != _player?.PlayerCombatState) return;
        Refresh();
    }

    private void Refresh()
    {
        if (_player == null || _label == null || _resource == null) return;
        var amount = _resource.Get(_player);

        _label.Text = amount.ToString();
        _label.AddThemeColorOverride("font_color",
            amount == 0 ? StsColors.red : new Color("EBFFAD"));
        _label.AddThemeColorOverride("font_outline_color",
            amount == 0 ? StsColors.unplayableEnergyCostOutline : new Color("3A3F2B"));

        var targetAlpha = amount > 0 ? 1f : 0f;

        if (!Visible && amount > 0)
        {
            Visible = true;
            Modulate = new Color(1, 1, 1, 0f);
            SlideIn();
        }

        _fadeTween?.Kill();
        _fadeTween = CreateTween();
        _fadeTween.TweenProperty(this, "modulate:a", targetAlpha, 0.3)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Cubic);

        if (amount == 0)
            _fadeTween.TweenCallback(Callable.From(() => Visible = false));
    }

    private void SlideIn()
    {
        _animOutTween?.Kill();
        _animInTween = CreateTween();
        Position = _hidePosition;
        _animInTween.TweenProperty(this, "position", _showPosition, 0.6)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Expo);
    }

    public void AnimIn() => SlideIn();

    public void AnimOut()
    {
        _animInTween?.Kill();
        _animOutTween = CreateTween();
        Position = _showPosition;
        _animOutTween.TweenProperty(this, "position", _hidePosition, 0.6)
            .SetEase(Tween.EaseType.In)
            .SetTrans(Tween.TransitionType.Back);
        _animOutTween.TweenCallback(Callable.From(() => Visible = false));
    }

    public static NCollectorEnergyCounter Create(Player player)
    {
        var scene = ResourceLoader.Load<PackedScene>("res://Collector/scenes/ui/collector_energy.tscn");
        var instance = scene.Instantiate<NCollectorEnergyCounter>();
        instance.Initialize(player);
        return instance;
    }

    private void OnHovered()
    {
        if (_hoverTip == null) return;
        NHoverTipSet.CreateAndShow(this, _hoverTip)?.SetGlobalPosition(GlobalPosition + new Vector2(-70f, -200f));
    }

    private void OnUnhovered() => NHoverTipSet.Remove(this);
}