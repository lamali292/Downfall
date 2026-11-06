using Godot;

namespace Hexaghost.HexaghostCode.Vfx;

public partial class FireballTrail : Line2D
{
    private const float MinSpawnDist = 8f;
    private const float MaxSpawnDist = 48f;

    private readonly List<float> _pointAge = [];
    private Vector2? _lastPointPosition;

    // Tune these to taste
    private float _pointDuration = 0.3f;
    public bool Emitting = true;
    public Node2D? Parent { get; set; }
    public Color Color { get; set; }

    public override void _Ready()
    {
        GlobalPosition = Vector2.Zero;
        GlobalRotation = 0f;

        Width = 12f;
        DefaultColor = Color;

        var gradient = new Gradient();
        gradient.SetColor(0, Color with { A = 0f });
        gradient.SetColor(1, Color with { A = 0.8f });
        Gradient = gradient;

        // Taper from wide at head to thin at tail
        var widthCurve = new Curve();
        widthCurve.AddPoint(new Vector2(0f, 0f)); // tail: thin
        widthCurve.AddPoint(new Vector2(0.5f, 1f)); // middle: wide
        widthCurve.AddPoint(new Vector2(1f, 0.6f)); // head: medium
        WidthCurve = widthCurve;
    }

    public override void _Process(double delta)
    {
        GlobalPosition = Vector2.Zero;
        GlobalRotation = 0f;

        var dt = (float)delta;

        // Age and remove old points
        for (var i = 0; i < GetPointCount(); i++)
        {
            _pointAge[i] += dt;
            if (!(_pointAge[i] > _pointDuration)) continue;
            RemovePoint(0);
            _pointAge.RemoveAt(0);
            i--;
        }

        if (Emitting && Parent != null)
            AddTrailPoint(Parent.GlobalPosition, dt);
    }

    private void AddTrailPoint(Vector2 pos, float delta)
    {
        if (_lastPointPosition.HasValue)
        {
            var dist = pos.DistanceTo(_lastPointPosition.Value);
            if (dist < MinSpawnDist) return;

            if (GetPointCount() > 2 && dist > MaxSpawnDist)
            {
                var prev2 = GetPointPosition(GetPointCount() - 2);
                var prev1 = GetPointPosition(GetPointCount() - 1);
                for (var d = MaxSpawnDist; d < dist - MinSpawnDist; d += MaxSpawnDist)
                {
                    var w = 0.5f + d / dist * 0.5f;
                    var interp = prev2.Lerp(prev1, w).Lerp(prev1.Lerp(pos, w), w);
                    var wobble = GetWobble(pos, _lastPointPosition.Value);
                    _pointAge.Add(delta * w);
                    AddPoint(interp + wobble);
                }
            }
        }

        var pointWobble = GetWobble(pos, _lastPointPosition ?? pos);
        _pointAge.Add(0f);
        AddPoint(pos + pointWobble);
        _lastPointPosition = pos;
    }

    private static Vector2 GetWobble(Vector2 pos, Vector2 lastPos)
    {
        var dir = (pos - lastPos).Normalized();
        var perp = new Vector2(-dir.Y, dir.X);
        var amount = (float)GD.RandRange(-6f, 6f);
        return perp * amount;
    }
}