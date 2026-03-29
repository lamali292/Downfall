using Godot;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

namespace Downfall.Code.Vfx;

public partial class NHemokinesisParticle : Sprite2D
{
    private Vector2 _targetPos;
    private Vector2 _lastPos; // Used for "Sweep" hit detection
    private float _speed = 1000f;
    private float _rotation;
    private float _rotationRate;
    private bool _rotateClockwise;
    private bool _lockedOn;
    private Line2D? _trail;

    private static readonly Texture2D SparkTex = 
        ResourceLoader.Load<Texture2D>("res://Downfall/vfx/glow_spark.png");

    public static NHemokinesisParticle Create(Vector2 start, Vector2 target)
    {
        return new NHemokinesisParticle {
            GlobalPosition = start,
            _lastPos = start,
            _targetPos = target,
            Texture = SparkTex,
            _rotation = (float)GD.RandRange(0, Mathf.Tau),
            _rotateClockwise = GD.RandRange(0, 1) == 0,
            _rotationRate = (float)GD.RandRange(600f, 650f),
            Modulate = new Color(1, 0, 0, 0.8f) 
        };
    }

    public override void _Ready()
    {
        // Trail Setup
        _trail = new Line2D {
            Texture = SparkTex,
            TextureMode = Line2D.LineTextureMode.Tile, 
            Width = 24f,
            DefaultColor = new Color(1, 0, 0, 0.6f), // This colors the white texture RED
            Material = new CanvasItemMaterial { BlendMode = CanvasItemMaterial.BlendModeEnum.Add },
            ZIndex = 1 
        };

        // Create the Taper (Scale from 0 to 1)
        var curve = new Curve();
        curve.AddPoint(new Vector2(0, 0)); // Tail tip
        curve.AddPoint(new Vector2(1, 1)); // Head connection
        _trail.WidthCurve = curve;

        GetParent()?.AddChild(_trail);
    }

    public override void _Process(double delta)
    {
        var fDelta = (float)delta;
        _lastPos = GlobalPosition; // Store position before moving
        
        var diff = _targetPos - GlobalPosition;
        if (!_lockedOn)
        {
            _rotationRate += fDelta * 2000f;
            var step = Mathf.DegToRad(_rotationRate) * fDelta;
            _rotation += _rotateClockwise ? step : -step;
            if (Mathf.Abs(Mathf.AngleDifference(_rotation, diff.Angle())) < step)
            {
                _rotation = diff.Angle();
                _lockedOn = true;
            }
        }
        else { _rotation = diff.Angle(); }
        GlobalRotation = _rotation;
        GlobalPosition += Vector2.FromAngle(_rotation) * _speed * fDelta;
        _speed = Mathf.MoveToward(_speed, 4000f, (_lockedOn ? 9000f : 4500f) * fDelta);
        _trail?.AddPoint(GlobalPosition);
        if (_trail != null && _trail.GetPointCount() > 20) _trail.RemovePoint(0);
        if (CheckHit())
        {
            OnHit();
        }
    }

    private bool CheckHit()
    {
        // Simple distance check first
        if (GlobalPosition.DistanceTo(_targetPos) < 42f) return true;

        var toTarget = _targetPos - _lastPos;
        var movement = GlobalPosition - _lastPos;
        
        if (!(movement.Length() > 0)) return false;
        var projection = toTarget.Dot(movement.Normalized());
        return projection > 0 && projection < movement.Length() && toTarget.Length() < 50f;
    }

    private void OnHit()
    {
        SfxPlayer.PlaySfx("res://Downfall/audio/heavy_blunt.ogg", (float)GD.RandRange(0.6f, 0.9f), 0.5f);
        NGame.Instance?.ScreenShake(ShakeStrength.Medium, ShakeDuration.Short);
        
        if (IsInstanceValid(_trail)) _trail.QueueFree();
        QueueFree();
    }
}