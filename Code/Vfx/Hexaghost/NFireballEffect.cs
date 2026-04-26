using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Downfall.Code.Vfx.Hexaghost;

public partial class NFireballEffect : Node2D
{
    private Vector2 _from;
    private Vector2 _target;
    private Sprite2D? _sprite;
    private CpuParticles2D? _trail;

    public static NFireballEffect Create(Vector2 from, Vector2 target)
    {
        var effect = new NFireballEffect();
        effect._from = from;
        effect._target = target + new Vector2(
            (float)GD.RandRange(-20f, 20f),
            (float)GD.RandRange(-20f, 20f)
        );
        return effect;
    }

    public override void _Ready()
    {
        GlobalPosition = _from;

        _sprite = new Sprite2D();
        _sprite.Texture = PreloadManager.Cache.GetTexture2D(
            "res://Downfall/images/vfx/glow_spark.png");
        AddChild(_sprite);

        /*
        _trail = new CpuParticles2D();
        _trail.Emitting = true;
        _trail.Amount = 20;
        _trail.Lifetime = 0.2f;
        _trail.Spread = 30f;
        _trail.InitialVelocityMin = 20f;
        _trail.InitialVelocityMax = 60f;
        _trail.Color = new Color(1f, 0.4f, 0f);
        AddChild(_trail);
        */
        var tween = CreateTween();
        
        var mid = _from.Lerp(_target, 0.5f) + Vector2.Up * 100f;
        tween.TweenProperty(this, "global_position", mid, 0.25f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(this, "global_position", _target, 0.25f)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.In);

        
        tween.Parallel().TweenProperty(_sprite, "scale", new Vector2(1.5f, 0.5f), 0.1f)
            .SetDelay(0.4f);
        
        var spinTween = CreateTween();
        spinTween.TweenProperty(_sprite, "rotation", Mathf.Tau, 0.5f);

        tween.TweenCallback(Callable.From(OnArrival));
        tween.TweenCallback(Callable.From(QueueFree));
    }

    private void OnArrival()
    {
      
        var container = NCombatRoom.Instance?.CombatVfxContainer;
        if (container == null) return;
        //VfxCmd.PlayVfx(GlobalPosition, NFireBurstVfx.scenePath, container);
        //VfxCmd.PlayVfx(GlobalPosition, NFireBurningVfx.scenePath, container);
        if (_trail != null) _trail.Emitting = false;
    }
}