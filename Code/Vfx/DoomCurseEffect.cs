using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.TestSupport;

namespace Downfall.Code.Vfx;

/// <summary>
/// Spawns a volley of stakes that materialize around the target, then fly in and pierce it.
///
/// Timeline per stake (total = StakeDuration):
///
///   [0 .. StakeDuration/2]   MATERIALIZE
///     Stake appears at spawn position, shrinking from large+transparent to full size+opaque.
///     Gives the player a moment to see where each stake is before it flies.
///
///   [StakeDuration/2 .. StakeDuration-SoundLeadIn]   FLIGHT
///     Stake accelerates toward the target using ExpOut (fast start, smooth arrival).
///     Stays fully opaque during flight.
///
///   [SoundLeadIn before arrival]   SOUND CUE
///     Impact sound plays slightly before arrival so it feels tight, not late.
///
///   [arrival]   IMPACT
///     Stake disappears. SpawnImpact() bursts splinter sprites outward.
/// </summary>
[GlobalClass]
public partial class DoomCurseEffect : Node2D
{
    // -------------------------------------------------------------------------
    // Timing
    // -------------------------------------------------------------------------

    /// Total lifetime of one stake from spawn to impact.
    private const float StakeDuration  = 0.6f;

    /// How far before arrival (in normalized flight progress 0..1) to play the sound.
    /// 0.8 = sound plays when stake is 80% of the way to the target.
    private const float SoundLeadIn    = 0.8f;

    /// Delay between spawning each stake in the volley.
    private const float SpawnInterval  = 0.04f;

    // -------------------------------------------------------------------------
    // Volley
    // -------------------------------------------------------------------------

    /// Total number of stakes in the volley.
    private const int StakeCount = 13;

    // -------------------------------------------------------------------------
    // Spawn position — where stakes appear before flying in
    // -------------------------------------------------------------------------

    /// Arc in degrees from which stakes can spawn around the target.
    /// -50..230 = 280° arc, leaving a gap at the bottom (matches original STS1).
    private const float SpawnAngleMin  = -50f;
    private const float SpawnAngleMax  = 230f;

    /// Distance range (in screen pixels) stakes spawn from the target, horizontal.
    private const float SpawnDistMin   = 200f;
    private const float SpawnDistMax   = 600f;

    /// Distance range (in screen pixels) stakes spawn from the target, vertical.
    private const float SpawnDistYMin  = 200f;
    private const float SpawnDistYMax  = 500f;

    /// Random offset applied to the target point so not all stakes hit the exact same pixel.
    private const float TargetScatterX = 50f;
    private const float TargetScatterY = 60f;

    // -------------------------------------------------------------------------
    // Stake visuals
    // -------------------------------------------------------------------------

    /// Size range of each stake sprite at rest.
    private const float StakeScaleMin              = 0.4f;
    private const float StakeScaleMax              = 1.1f;

    /// Alpha of the stake during flight.
    private const float StakeAlpha                 = 0.8f;

    /// How much bigger the stake appears during the materialize phase (shrinks down from this).
    private const float StakeMaterializeScaleMult  = 10f;

    // -------------------------------------------------------------------------
    // Impact burst
    // -------------------------------------------------------------------------

    /// Number of splinter sprites spawned at impact point.
    private const int   ImpactSpriteCount   = 5;
    private const float ImpactScaleMin      = 0.2f;
    private const float ImpactScaleMax      = 0.5f;

    /// How long impact splinters take to fade out.
    private const float ImpactFadeDuration  = 0.3f;

    // -------------------------------------------------------------------------
    // Stake color (purple/pink tones — additive blend, so low G keeps it lively)
    // -------------------------------------------------------------------------

    private const float ColorRMin = 0.5f;
    private const float ColorRMax = 1.0f;
    private const float ColorGMin = 0.0f;
    private const float ColorGMax = 0.4f;
    private const float ColorBMin = 0.5f;
    private const float ColorBMax = 1.0f;

    // -------------------------------------------------------------------------
    // Sound
    // -------------------------------------------------------------------------

    private string SfxIntro  = "event:/sfx/characters/necrobinder/necrobinder_doom_kill";
    private string SfxImpact = "event:/sfx/characters/silent/silent_attack";
    private float  SfxImpactVolume = 0.3f;

    // -------------------------------------------------------------------------
    // Internal state
    // -------------------------------------------------------------------------

    private struct Stake
    {
        public Sprite2D Sprite;
        public float    Duration;
        public float    StartX,  StartY;
        public float    TargetX, TargetY;
        public float    StartAngle, TargetAngle;
        public float    TargetScale;
        public Color    Color;
        public bool     SoundPlayed;
        public bool     ImpactSpawned;
    }

    private static Texture2D?          _stakeTex;
    private static CanvasItemMaterial? _additiveMat;

    private readonly List<Stake>     _stakes = new();
    private CancellationTokenSource? _cts;
    private float                    _cx, _cy;
    private int                      _count = StakeCount;
    private bool                     _started;

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

    // -------------------------------------------------------------------------
    // Lifecycle
    // -------------------------------------------------------------------------

    public override void _Ready()
    {
        _stakeTex    ??= PreloadManager.Cache.GetAsset<Texture2D>("res://Downfall/images/vfx/stake.png");
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

    // -------------------------------------------------------------------------
    // Spawner coroutine — fires stakes one by one then waits for them to finish
    // -------------------------------------------------------------------------

    private async Task PlaySequence(CancellationToken ct)
    {
        while (_count > 0 && !ct.IsCancellationRequested)
        {
            if (!_started)
            {
                _started = true;
                SfxCmd.Play(SfxIntro);

                var overlay = NDoomOverlayVfx.GetOrCreate();
                if (overlay != null)
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(overlay);

                ScreenFlashEffect.Play(new Color(1f, 0f, 1f, 0.7f));
            }

            SpawnStake();
            _count--;

            await Cmd.Wait(SpawnInterval, ct);
        }

        // Wait for in-flight stakes to finish before freeing the node
        while (_stakes.Count > 0 && !ct.IsCancellationRequested)
            await Cmd.Wait(SpawnInterval, ct);

        this.QueueFreeSafely();
    }

    // -------------------------------------------------------------------------
    // Per-frame update — drives the three-phase timeline for each stake
    // -------------------------------------------------------------------------

    public override void _Process(double delta)
    {
        var dt       = (float)delta;
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
                // ---- PHASE 1: MATERIALIZE ----------------------------------------
                // Stake sits at spawn position, shrinking from large+invisible to normal size+opaque.
                case > half:
                {
                    var p = (s.Duration - half) / half; // 1 → 0 as time passes

                    s.Sprite.GlobalPosition = new Vector2(s.StartX, s.StartY);
                    s.Sprite.Scale          = Vector2.One * Mathf.Lerp(s.TargetScale, s.TargetScale * StakeMaterializeScaleMult, ElasticIn(p));
                    s.Sprite.Modulate       = new Color(s.Color.R, s.Color.G, s.Color.B, FadeLerp(StakeAlpha, 0f, p));
                    break;
                }
                // ---- PHASE 2: FLIGHT ---------------------------------------------
                // Stake accelerates toward target. Sound fires when 80% of the way there.
                default:
                {
                    var p = ExpOut(1f - s.Duration / half); // 0 → 1 as stake approaches

                    // Sound cue slightly before arrival so it feels tight
                    if (p >= SoundLeadIn && !s.SoundPlayed)
                    {
                        SfxCmd.Play(SfxImpact, SfxImpactVolume);
                        s.SoundPlayed = true;
                    }

                    s.Sprite.GlobalPosition = new Vector2(
                        Mathf.Lerp(s.StartX, s.TargetX, p),
                        Mathf.Lerp(s.StartY, s.TargetY, p)
                    );
                    s.Sprite.Scale    = Vector2.One * s.TargetScale;
                    s.Sprite.Modulate = new Color(s.Color.R, s.Color.G, s.Color.B, StakeAlpha);

                    // ---- PHASE 3: IMPACT -----------------------------------------
                    // Stake reaches target, disappears, splinters burst outward.
                    if (p >= 0.99f && !s.ImpactSpawned)
                    {
                        //SpawnImpact(s.TargetX, s.TargetY, s.Color);
                        s.Sprite.QueueFreeSafely();
                        _stakes.RemoveAt(i);
                        continue;
                    }

                    break;
                }
            }

            // Rotation interpolates throughout full lifetime
            s.Sprite.RotationDegrees = Mathf.Lerp(s.TargetAngle, s.StartAngle, ElasticIn(1f - s.Duration / StakeDuration));
            _stakes[i] = s;
        }
    }

    // -------------------------------------------------------------------------
    // Spawn helpers
    // -------------------------------------------------------------------------

    private void SpawnStake()
    {
        // Divide distances by scene scale so pixel distances look consistent
        // regardless of zoom level (same pattern as NDoomVfx)
        var sceneScale = NCombatRoom.Instance?.SceneContainer.Scale.X ?? 1f;
        var rng        = new RandomNumberGenerator();
        rng.Randomize();

        var angle = rng.RandfRange(SpawnAngleMin, SpawnAngleMax) * Mathf.Pi / 180f;

        // Target: center of creature + small random scatter
        var tx = _cx + rng.RandfRange(-TargetScatterX, TargetScatterX) / sceneScale;
        var ty = _cy + rng.RandfRange(-TargetScatterY, TargetScatterY) / sceneScale;

        // Spawn: offset outward along the random angle
        var sx = Mathf.Cos(angle) * rng.RandfRange(SpawnDistMin,  SpawnDistMax)  / sceneScale + tx;
        var sy = Mathf.Sin(angle) * rng.RandfRange(SpawnDistYMin, SpawnDistYMax) / sceneScale + ty;

        var color = new Color(
            rng.RandfRange(ColorRMin, ColorRMax),
            rng.RandfRange(ColorGMin, ColorGMax),
            rng.RandfRange(ColorBMin, ColorBMax)
        );

        var sprite = new Sprite2D
        {
            Texture         = _stakeTex,
            Material        = _additiveMat,
            RotationDegrees = rng.RandfRange(0f, 360f),
            Scale           = Vector2.One * 0.01f,
            Modulate        = new Color(color.R, color.G, color.B, 0f)
        };
        AddChild(sprite);
        sprite.GlobalPosition = new Vector2(sx, sy); // must be after AddChild

        _stakes.Add(new Stake
        {
            Sprite      = sprite,
            Duration    = StakeDuration,
            StartX      = sx,  StartY      = sy,
            TargetX     = tx,  TargetY     = ty,
            StartAngle  = sprite.RotationDegrees,
            // +180 flips the stake to point toward the target rather than away
            TargetAngle = Mathf.Atan2(ty - sy, tx - sx) * 180f / Mathf.Pi + 90f + 180f,
            TargetScale = rng.RandfRange(StakeScaleMin, StakeScaleMax),
            Color       = color
        });
    }
    
    // -------------------------------------------------------------------------
    // Easing functions (standard formulas, not tunable constants)
    // -------------------------------------------------------------------------

    /// Elastic ease-in: overshoots then snaps — used for materialize shrink and rotation.
    private static float ElasticIn(float t)
    {
        if (t is 0f or 1f) return t;
        return Mathf.Pow(2f, 10f * (t - 1f)) * Mathf.Sin((t - 1.1f) * (2f * Mathf.Pi) / 0.4f);
    }

    /// Exponential ease-out: fast start, smooth arrival — used for stake flight.
    private static float ExpOut(float t) =>
        t == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t);

    /// Smoothstep lerp (Ken Perlin's fade): zero velocity at both ends — used for alpha fades.
    private static float FadeLerp(float a, float b, float t)
    {
        t = Mathf.Clamp(t, 0f, 1f);
        t = t * t * t * (t * (t * 6f - 15f) + 10f);
        return a + (b - a) * t;
    }
}