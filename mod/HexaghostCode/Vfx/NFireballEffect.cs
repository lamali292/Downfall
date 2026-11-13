using Godot;
using MegaCrit.Sts2.Core.Assets;

namespace Hexaghost.HexaghostCode.Vfx;

public partial class NFireballEffect : Node2D
{
    private bool _arrived;
    private Color _color;
    private Vector2 _control;

    private float _duration = 0.5f;
    private float _elapsed;
    private CpuParticles2D? _fire;
    private Vector2 _from;
    private CpuParticles2D? _sparks;
    private Vector2 _target;
    private FireballTrail? _trail;

    public static NFireballEffect Create(Vector2 from, Vector2 target, Color fireColor)
    {
        var effect = new NFireballEffect();
        effect._from = from;
        effect._color = fireColor;
        effect._target = target + new Vector2(
            (float)GD.RandRange(-20f, 20f),
            (float)GD.RandRange(-20f, 20f)
        );
        effect._control = from.Lerp(effect._target, 0.5f) + Vector2.Up * 300f;
        return effect;
    }

    public override void _Ready()
    {
        GlobalPosition = _from;

        var gradient = new Gradient();
        gradient.SetColor(0, _color with { A = 1f });
        gradient.SetColor(1, _color with { A = 0f });

        var scaleCurve = new Curve();
        scaleCurve.AddPoint(new Vector2(0f, 1f));
        scaleCurve.AddPoint(new Vector2(1f, 0f));

        var fireMaterial = new CanvasItemMaterial();
        fireMaterial.BlendMode = CanvasItemMaterial.BlendModeEnum.Add;
        fireMaterial.ParticlesAnimation = true;
        fireMaterial.ParticlesAnimHFrames = 4;
        fireMaterial.ParticlesAnimVFrames = 1;
        fireMaterial.ParticlesAnimLoop = false;

        _fire = new CpuParticles2D();
        _fire.Material = fireMaterial;
        _fire.AnimOffsetMax = 1f;
        _fire.Amount = 10;
        _fire.Lifetime = 0.3f;
        _fire.SpeedScale = 2f;
        _fire.LocalCoords = false;
        _fire.EmissionShape = CpuParticles2D.EmissionShapeEnum.Sphere;
        _fire.EmissionSphereRadius = 4f;
        _fire.Direction = Vector2.Zero;
        _fire.Spread = 180f;
        _fire.Gravity = Vector2.Zero;
        _fire.InitialVelocityMin = 20f;
        _fire.InitialVelocityMax = 60f;
        _fire.ScaleAmountMin = 0.3f;
        _fire.ScaleAmountMax = 0.6f;
        _fire.ScaleAmountCurve = scaleCurve;
        _fire.ColorRamp = gradient;
        _fire.Texture = PreloadManager.Cache.GetTexture2D(
            "res://images/vfx/vfx_constant_fire/fire_texture_2.png");
        AddChild(_fire);

        _sparks = new CpuParticles2D();
        _sparks.Amount = 10;
        _sparks.Lifetime = 0.4f;
        _sparks.SpeedScale = 2f;
        _sparks.LocalCoords = false;
        _sparks.Direction = Vector2.Zero;
        _sparks.Spread = 180f;
        _sparks.Gravity = Vector2.Zero;
        _sparks.InitialVelocityMin = 40f;
        _sparks.InitialVelocityMax = 100f;
        _sparks.ScaleAmountMin = 0.05f;
        _sparks.ScaleAmountMax = 0.15f;
        _sparks.ColorRamp = gradient;
        _sparks.Texture = PreloadManager.Cache.GetTexture2D(
            "res://images/vfx/vfx_constant_fire/fire_spark.png");
        AddChild(_sparks);

        _trail = new FireballTrail();
        _trail.Parent = this;
        _trail.Color = _color;
        AddChild(_trail);
    }


    public override void _Process(double delta)
    {
        if (_arrived) return;

        _elapsed += (float)delta;
        var t = Mathf.Clamp(_elapsed / _duration, 0f, 1f);
        var et = Mathf.SmoothStep(0f, 1f, t);

        GlobalPosition = _from.Lerp(_control, et).Lerp(_control.Lerp(_target, et), et);

        if (t < 1f)
        {
            var dir = (_control.Lerp(_target, et) - _from.Lerp(_control, et)).Normalized();

            // Point particles backwards so they trail behind
            if (_fire != null)
            {
                _fire.Direction = -dir;
                _fire.Spread = 30f;
            }

            if (_sparks == null) return;
            _sparks.Direction = -dir;
            _sparks.Spread = 20f;
        }
        else
        {
            OnArrival();
        }
    }

    private void OnArrival()
    {
        _arrived = true;
        if (_trail != null) _trail.Emitting = false;
        if (_fire != null) _fire.Emitting = false;
        if (_sparks != null) _sparks.Emitting = false;
        GetTree().CreateTimer(0.4f).Timeout += QueueFree;
    }
}