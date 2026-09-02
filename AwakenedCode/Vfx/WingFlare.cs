using Godot;

namespace Awakened.AwakenedCode.Vfx;

[GlobalClass]
public partial class WingFlare : Node2D
{
    private const float SpawnInterval = 0.1f;
    private const float SizeMultiplier = 0.8f;
    private static Texture2D? _spikeTex;
    private readonly List<Spike> _spikes = new();

    private bool _active;
    private float _spawnTimer;
    private static Texture2D SpikeTex => _spikeTex ??= GD.Load<Texture2D>("res://Awakened/images/character/spike.png");

    public void SetActive(bool on)
    {
        _active = on;
    }

    public override void _Process(double delta)
    {
        var dt = (float)delta;

        if (_active)
        {
            _spawnTimer -= dt;

            if (_spawnTimer <= 0f)
            {
                _spawnTimer = SpawnInterval;
                SpawnSpike();
            }
        }

        for (var i = _spikes.Count - 1; i >= 0; i--)
        {
            var s = _spikes[i];

            s.Duration -= dt;

            if (s.Duration <= 0f)
            {
                s.Glow.QueueFree();
                s.Main.QueueFree();
                s.Shadow.QueueFree();

                _spikes.RemoveAt(i);
                continue;
            }

            UpdateSpike(s);
        }
    }

    private void SpawnSpike()
    {
        float x;
        float y;
        float targetScale;

        var roll = GD.RandRange(0, 2);

        switch (roll)
        {
            case 0:
                x = (float)GD.RandRange(-340.0, -170.0);
                y = (float)GD.RandRange(-20.0, 20.0);
                targetScale = (float)GD.RandRange(0.4, 0.5);
                break;
            case 1:
                x = (float)GD.RandRange(-220.0, -20.0);
                y = (float)GD.RandRange(-40.0, -10.0);
                targetScale = (float)GD.RandRange(0.4, 0.5);
                break;
            default:
                x = (float)GD.RandRange(-270.0, -60.0);
                y = (float)GD.RandRange(-30.0, 0.0);
                targetScale = (float)GD.RandRange(0.4, 0.7);
                break;
        }

        x += 155f;
        y += 30f;

        float width = SpikeTex.GetWidth();
        float height = SpikeTex.GetHeight();

        x -= width / 2f;
        y -= height / 2f;

        x = -x;
        y = -y;

        var pos = new Vector2(x, y);

        var baseRot = (float)GD.RandRange(25.0, 85.0);
        var colorA = (float)GD.RandRange(0.5, 0.9);

        var spike = new Spike
        {
            Glow = MakeLayer(pos, true),
            Main = MakeLayer(pos, false),
            Shadow = MakeLayer(pos, false),
            Duration = 2.0f,
            TargetScale = targetScale * SizeMultiplier,
            BaseRotation = baseRot,
            ColorA = colorA
        };

        AddChild(spike.Glow);
        AddChild(spike.Main);
        AddChild(spike.Shadow);

        _spikes.Add(spike);

        UpdateSpike(spike);
    }

    private static Sprite2D MakeLayer(Vector2 pos, bool additive)
    {
        var sprite = new Sprite2D
        {
            Texture = SpikeTex,
            Centered = true,
            Position = pos
        };

        float width = SpikeTex.GetWidth();

        sprite.Offset = new Vector2(width * 0.5f - width * 0.08f, 0f);

        if (additive)
            sprite.Material = new CanvasItemMaterial
            {
                BlendMode = CanvasItemMaterial.BlendModeEnum.Add
            };

        return sprite;
    }

    private static void UpdateSpike(Spike s)
    {
        float scale;

        if (s.Duration > 1.0f)
        {
            var t = s.Duration - 1.0f;
            scale = BounceIn(s.TargetScale, 0.01f * SizeMultiplier, t);
        }
        else
        {
            scale = s.TargetScale;
        }

        var a = s.ColorA;

        if (s.Duration < 0.2f) a = Mathf.Lerp(0f, 0.5f, s.Duration * 5f);

        var derp = (float)GD.RandRange(3.0, 5.0);
        var rot = s.BaseRotation + derp;

        s.Glow.Scale = new Vector2(-scale * (float)GD.RandRange(1.1, 1.25), scale);
        s.Glow.RotationDegrees = rot;
        s.Glow.Modulate = new Color(0.4f, 1.0f, 1.0f, a / 2f);

        s.Main.Scale = new Vector2(-scale, scale);
        s.Main.RotationDegrees = rot;
        s.Main.Modulate = new Color(0.3f, 0.3f, 0.34f, a);

        s.Shadow.Scale = new Vector2(-scale * 0.7f, scale * 0.7f);
        s.Shadow.RotationDegrees = rot - 40.0f;
        s.Shadow.Modulate = new Color(0f, 0f, 0f, a / 5f);
    }

    private static float BounceIn(float start, float end, float t)
    {
        t = Mathf.Clamp(t, 0f, 1f);
        var b = 1f - t;
        var bounce = Mathf.Abs(Mathf.Sin(b * Mathf.Pi * 2.5f)) * (1f - b);

        return Mathf.Lerp(start, end, bounce);
    }

    private sealed class Spike
    {
        public float BaseRotation;
        public float ColorA;

        public float Duration;
        public Sprite2D Glow = null!;
        public Sprite2D Main = null!;
        public Sprite2D Shadow = null!;
        public float TargetScale;
    }
}