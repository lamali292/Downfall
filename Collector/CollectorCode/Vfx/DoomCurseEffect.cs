using Downfall.DownfallCode.Vfx;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.TestSupport;

namespace Collector.CollectorCode.Vfx;

[GlobalClass]
public partial class DoomCurseEffect : Node2D
{
    private const float StakeDuration = 0.6f;
    private const float SoundLeadIn = 0.8f;
    private const float SpawnInterval = 0.04f;
    private const int StakeCount = 13;


    private const float SpawnAngleMin = -50f;
    private const float SpawnAngleMax = 230f;
    private const float SpawnDistMin = 200f;
    private const float SpawnDistMax = 600f;
    private const float SpawnDistYMin = 200f;
    private const float SpawnDistYMax = 500f;
    private const float TargetScatterX = 50f;
    private const float TargetScatterY = 60f;
    private const float StakeScaleMin = 0.4f;
    private const float StakeScaleMax = 1.1f;

    private const float StakeAlpha = 0.8f;
    private const float StakeMaterializeScaleMult = 10f;

    private const float ColorRMin = 0.5f;
    private const float ColorRMax = 1.0f;
    private const float ColorGMin = 0.0f;
    private const float ColorGMax = 0.4f;
    private const float ColorBMin = 0.5f;
    private const float ColorBMax = 1.0f;

    private static Texture2D? _stakeTex;
    private static CanvasItemMaterial? _additiveMat;

    private readonly List<Stake> _stakes = [];
    private int _count = StakeCount;
    private CancellationTokenSource? _cts;
    private float _cx, _cy;
    private string _sfxImpact = "event:/sfx/characters/silent/silent_attack";
    private float _sfxImpactVolume = 0.3f;

    private string _sfxIntro = "event:/sfx/characters/necrobinder/necrobinder_doom_kill";
    private bool _started;

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    public static DoomCurseEffect? Create(Creature target)
    {
        if (TestMode.IsOn) return null;

        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(target);
        if (creatureNode == null) return null;

        var pos = creatureNode.VfxSpawnPosition;
        return new DoomCurseEffect { _cx = pos.X, _cy = pos.Y };
    }

    public override void _Ready()
    {
        _stakeTex ??= PreloadManager.Cache.GetAsset<Texture2D>("res://Downfall/images/vfx/stake.png");
        _additiveMat ??= new CanvasItemMaterial { BlendMode = CanvasItemMaterial.BlendModeEnum.Add };

        _cts = new CancellationTokenSource();
        TaskHelper.RunSafely(PlaySequence(_cts.Token));
    }

    public override void _ExitTree()
    {
        _cts?.Cancel();
        _cts?.Dispose();

        foreach (var s in _stakes)
            s.Sprite.QueueFreeSafely();
        _stakes.Clear();
    }

    private async Task PlaySequence(CancellationToken ct)
    {
        while (_count > 0 && !ct.IsCancellationRequested)
        {
            if (!_started)
            {
                _started = true;
                SfxCmd.Play(_sfxIntro);

                var overlay = NDoomOverlayVfx.GetOrCreate();
                if (overlay != null && overlay.GetParent() == null)
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(overlay);

                ScreenFlashEffect.Play(new Color(1f, 0f, 1f, 0.7f));
            }

            SpawnStake();
            _count--;

            await Cmd.Wait(SpawnInterval, ct);
        }

        while (_stakes.Count > 0 && !ct.IsCancellationRequested)
            await Cmd.Wait(SpawnInterval, ct);

        this.QueueFreeSafely();
    }

    public override void _Process(double delta)
    {
        var dt = (float)delta;
        const float half = StakeDuration * 0.5f;

        for (var i = _stakes.Count - 1; i >= 0; i--)
        {
            var s = _stakes[i];
            s.Duration -= dt;

            switch (s.Duration)
            {
                case <= 0f:
                    s.Sprite.QueueFreeSafely();
                    _stakes.RemoveAt(i);
                    continue;
                case > half:
                {
                    var p = (s.Duration - half) / half; // 1 → 0 as time passes

                    s.Sprite.GlobalPosition = new Vector2(s.StartX, s.StartY);
                    s.Sprite.Scale = Vector2.One * Mathf.Lerp(s.TargetScale, s.TargetScale * StakeMaterializeScaleMult,
                        ElasticIn(p));
                    s.Sprite.Modulate = new Color(s.Color.R, s.Color.G, s.Color.B, FadeLerp(StakeAlpha, 0f, p));
                    break;
                }
                default:
                {
                    var p = ExpOut(1f - s.Duration / half); // 0 → 1 as stake approaches
                    if (p >= SoundLeadIn && !s.SoundPlayed)
                    {
                        SfxCmd.Play(_sfxImpact, _sfxImpactVolume);
                        s.SoundPlayed = true;
                    }

                    s.Sprite.GlobalPosition = new Vector2(
                        Mathf.Lerp(s.StartX, s.TargetX, p),
                        Mathf.Lerp(s.StartY, s.TargetY, p)
                    );
                    s.Sprite.Scale = Vector2.One * s.TargetScale;
                    s.Sprite.Modulate = new Color(s.Color.R, s.Color.G, s.Color.B, StakeAlpha);

                    if (p >= 0.99f)
                    {
                        s.Sprite.QueueFreeSafely();
                        _stakes.RemoveAt(i);
                        continue;
                    }

                    break;
                }
            }

            s.Sprite.RotationDegrees =
                Mathf.Lerp(s.TargetAngle, s.StartAngle, ElasticIn(1f - s.Duration / StakeDuration));
            _stakes[i] = s;
        }
    }

    private void SpawnStake()
    {
        var sceneScale = NCombatRoom.Instance?.SceneContainer.Scale.X ?? 1f;
        var rng = new RandomNumberGenerator();
        rng.Randomize();

        var angle = rng.RandfRange(SpawnAngleMin, SpawnAngleMax) * Mathf.Pi / 180f;

        var tx = _cx + rng.RandfRange(-TargetScatterX, TargetScatterX) / sceneScale;
        var ty = _cy + rng.RandfRange(-TargetScatterY, TargetScatterY) / sceneScale;

        var sx = Mathf.Cos(angle) * rng.RandfRange(SpawnDistMin, SpawnDistMax) / sceneScale + tx;
        var sy = Mathf.Sin(angle) * rng.RandfRange(SpawnDistYMin, SpawnDistYMax) / sceneScale + ty;

        var color = new Color(
            rng.RandfRange(ColorRMin, ColorRMax),
            rng.RandfRange(ColorGMin, ColorGMax),
            rng.RandfRange(ColorBMin, ColorBMax)
        );

        var sprite = new Sprite2D
        {
            Texture = _stakeTex,
            Material = _additiveMat,
            RotationDegrees = rng.RandfRange(0f, 360f),
            Scale = Vector2.One * 0.01f,
            Modulate = new Color(color.R, color.G, color.B, 0f)
        };
        AddChild(sprite);
        sprite.GlobalPosition = new Vector2(sx, sy); // must be after AddChild

        _stakes.Add(new Stake
        {
            Sprite = sprite,
            Duration = StakeDuration,
            StartX = sx, StartY = sy,
            TargetX = tx, TargetY = ty,
            StartAngle = sprite.RotationDegrees,
            TargetAngle = Mathf.Atan2(ty - sy, tx - sx) * 180f / Mathf.Pi + 90f + 180f,
            TargetScale = rng.RandfRange(StakeScaleMin, StakeScaleMax),
            Color = color
        });
    }

    private static float ElasticIn(float t)
    {
        if (t is 0f or 1f) return t;
        return Mathf.Pow(2f, 10f * (t - 1f)) * Mathf.Sin((t - 1.1f) * (2f * Mathf.Pi) / 0.4f);
    }

    private static float ExpOut(float t)
    {
        return t == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t);
    }

    private static float FadeLerp(float a, float b, float t)
    {
        t = Mathf.Clamp(t, 0f, 1f);
        t = t * t * t * (t * (t * 6f - 15f) + 10f);
        return a + (b - a) * t;
    }

    // -------------------------------------------------------------------------
    // Internal state
    // -------------------------------------------------------------------------

    private struct Stake
    {
        public Sprite2D Sprite;
        public float Duration;
        public float StartX, StartY;
        public float TargetX, TargetY;
        public float StartAngle, TargetAngle;
        public float TargetScale;
        public Color Color;
        public bool SoundPlayed;
    }
}