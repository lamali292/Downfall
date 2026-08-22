using BaseLib.Utils;
using Downfall.DownfallCode.Utils.UI;
using Godot;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Hexaghost.HexaghostCode.Vfx;

// ─────────────────────────────────────────────────────────────────────────────
//  Pure layout math — no node state, no side effects. Safe to unit-test and
//  impossible to get into a bad runtime state. Everything positional lives here.
// ─────────────────────────────────────────────────────────────────────────────
internal static class GhostflameLayout
{
    public const float WheelRadius = 140f;

    public static readonly Vector2 FireScale = new(1, 1);
    public static readonly Vector2 ReticleVisualSize = new(44, 44);

    // Fire sprites are positioned with their origin at the base, not their visual
    // centre — without this the bracket reads as centred too low.
    public static readonly Vector2 ReticleCenterOffset = new(0, -22);

    public static readonly Vector2 HitboxSize = new(80, 80);

    /// Position of fire <paramref name="index"/> on a wheel of <paramref name="count"/> flames.
    public static Vector2 FirePosition(int index, int count)
    {
        if (count <= 0) return Vector2.Zero;
        var angle = Mathf.Pi / 2f + Mathf.Pi / count - index * Mathf.Tau / count;
        return new Vector2(
            WheelRadius * Mathf.Cos(angle),
            -WheelRadius * Mathf.Sin(angle)); // screen y is down
    }

    /// Target wheel rotation that brings <paramref name="fireIndex"/> to the top.
    public static double WheelRotation(int fireIndex, int count)
    {
        if (count <= 0) return 0.0;
        return -(fireIndex - 0.5) * Mathf.Tau / count;
    }

    /// Shortest-path rotation from <paramref name="current"/> to the target for a flame.
    public static float ShortestRotationTo(float current, double target)
    {
        var diff = Mathf.AngleDifference(current, (float)target);
        return current + diff;
    }

    public static float IntentAlpha(int index, int currentIndex) => index == currentIndex ? 1f : 0f;

    /// Where a fire's intent icon sits relative to the fire, given the current scale.
    public static Vector2 IntentOffset(float scaleX, float scaleY)
        => Vector2.Up * 130f * scaleY + Vector2.Left * 33f * scaleX;

    /// Fallback ring centre when the HexaghostScene node can't be found.
    public static Vector2 FallbackCenter(Vector2 creatureGlobal, float scaleY)
        => creatureGlobal + Vector2.Up * 170f * scaleY;

    public static float ExtraScale(float creatureScale, float containerScale, float tempScale)
    {
        if (Mathf.IsZeroApprox(containerScale)) return tempScale; // avoid div-by-zero
        return tempScale * Mathf.Abs(creatureScale / containerScale);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
//  Node. Stateful by nature, but every dangerous transition is either made
//  idempotent or logged. See the block comment on each guard.
// ─────────────────────────────────────────────────────────────────────────────
public partial class NGhostflames : Control
{
    private static string FireScenePath => "res://Hexaghost/scenes/character/hexaghost_flame.tscn";

    // Built lazily/idempotently from the wheel length, never assumed to be 6.
    private NFire?[] _fires = [];
    private NIntent?[] _intents = [];
    private Node2D?[] _hitboxAnchors = [];
    private Control?[] _hitboxes = [];
    private NSelectionReticle?[] _reticles = [];
    private List<Control> _reachableHitboxes = [];

    private NCreature? _creatureNode;
    private Node2D? _hexaCenter; // %HexaghostScene inside the creature visuals — the ring's centre
    private Control? _vfxContainer;
    private GhostflameModel[]? _currentWheel;
    private Player? _player;

    private Tween? _positionTween;
    private Tween? _intentTween;
    private Tween? _fadeTween;

    private PackedScene? _fireScene;
    private bool _dead;            // FadeOutOnDeath ran and no revive yet — suppresses invariant reset
    private bool _loggedTrackState;
    private double _ungatedProcessTime; // seconds _Process has early-returned for want of Track

    private ulong Id => GetInstanceId();

    // ── Construction ─────────────────────────────────────────────────────────

    public static NGhostflames Create(Player player)
    {
        var root = new NGhostflames
        {
            Name = "Ghostflames",
            _player = player,
            ZIndex = 0,
            ZAsRelative = false
        };

        root._fireScene = ResourceLoader.Load<PackedScene>(FireScenePath);
        if (root._fireScene == null)
            HexaghostMainFile.Logger.Error($"[Ghostflames #{root.Id}] failed to load {FireScenePath}");

        var count = HexaghostModel.Wheel.Get(player)?.Length ?? 0;
        HexaghostMainFile.Logger.Info($"[Ghostflames #{root.Id}] Create: wheel count={count}");
        
        root.EnsureBuilt(count);
        return root;
    }

    public override void _Ready()
    {
        EnsureBuilt(_fires.Length);
    }

    // ── Idempotent build ───────────────────────────────────────────────────────
    // Called from Create, _Ready, and RefreshWheel. Rebuilds only when the count
    // actually changes, so calling it every frame would be cheap and harmless.
    // This is what makes the "count==0 at Create → invisible forever" bug impossible.
    private void EnsureBuilt(int count)
    {
        if (count <= 0)
        {
            if (_fires.Length == 0)
                HexaghostMainFile.Logger.Warn(
                    $"[Ghostflames #{Id}] EnsureBuilt(0): no flames to build yet (model not ready?)");
            return;
        }

        if (_fires.Length == count && _fires.All(f => f != null && IsInstanceValid(f)))
            return; // already correct
        
        TearDownBuilt();

        _fireScene ??= ResourceLoader.Load<PackedScene>(FireScenePath);
        if (_fireScene == null)
        {
            HexaghostMainFile.Logger.Error($"[Ghostflames #{Id}] EnsureBuilt: fire scene unavailable");
            return;
        }

        _fires = new NFire?[count];
        _intents = new NIntent?[count];
        _hitboxAnchors = new Node2D?[count];
        _hitboxes = new Control?[count];
        _reticles = new NSelectionReticle?[count];

        for (var i = 0; i < count; i++)
        {
            var fire = _fireScene.Instantiate<NFire>();
            fire.Name = $"fire{i + 1}";
            fire.Position = GhostflameLayout.FirePosition(i, count);
            fire.Scale = GhostflameLayout.FireScale;
            AddChild(fire);
            _fires[i] = fire;

            BuildScaffolding(i);
        }

        _reachableHitboxes = _hitboxes.Where(h => h != null).Cast<Control>().ToList();
        DownfallControllerNav.WireChain(_reachableHitboxes, true);

        // A fresh build is a fresh combat presentation — clear any stale death state.
        ResetVisibilityInvariants(force: true);

        // Re-link controller nav if we already know the creature.
        if (_creatureNode != null && IsInstanceValid(_creatureNode))
            DownfallControllerNav.LinkAbove(_reachableHitboxes, _creatureNode.Hitbox);
    }

    private void BuildScaffolding(int i)
    {
        var intent = NIntent.Create(i * 0.3f);
        intent.Visible = false;
        intent.MouseFilter = MouseFilterEnum.Ignore;
        AddChild(intent);
        _intents[i] = intent;

        var anchor = new Node2D();
        AddChild(anchor);
        _hitboxAnchors[i] = anchor;

        var hitbox = new Control
        {
            CustomMinimumSize = GhostflameLayout.HitboxSize,
            MouseFilter = MouseFilterEnum.Stop
        };
        hitbox.Position = -hitbox.CustomMinimumSize / 2f;
        anchor.AddChild(hitbox);
        _hitboxes[i] = hitbox;

        // Drop focus on mouse-up so the hover tip is driven purely by mouse enter/exit.
        // (A latched focus otherwise keeps the tip up and blocks onFocus re-firing.)
        var hb = hitbox;
        hb.GuiInput += ev =>
        {
            if (ev is InputEventMouseButton { Pressed: false })
                hb.ReleaseFocus();
        };

        var reticle = DownfallControllerNav.AttachFocusReticle(
            anchor, GhostflameLayout.ReticleCenterOffset, GhostflameLayout.ReticleVisualSize, 4f);
        _reticles[i] = reticle;

        var index = i;
        DownfallControllerNav.WireHover(hitbox,
            () =>
            {
                if (NControllerManager.Instance?.IsUsingButtonInputsCompatibility() == true)
                    reticle?.OnSelect();
                var flame = _currentWheel?.ElementAtOrDefault(index);
                if (flame == null || _player == null) return;
                NCombatRoom.Instance?.GetCreatureNode(_player.Creature)?.ShowHoverTips(flame.HoverTips);
            },
            () =>
            {
                reticle?.OnDeselect();
                if (_player == null) return;
                NCombatRoom.Instance?.GetCreatureNode(_player.Creature)?.HideHoverTips();
            });
    }

    private void TearDownBuilt()
    {
        foreach (var n in _fires) n?.QueueFree();
        foreach (var n in _intents) n?.QueueFree();
        foreach (var n in _hitboxAnchors) n?.QueueFree(); // reticle + hitbox are children, freed with it

        _fires = [];
        _intents = [];
        _hitboxAnchors = [];
        _hitboxes = [];
        _reticles = [];
        _reachableHitboxes = [];
    }

    // ── Visibility invariants ──────────────────────────────────────────────────
    // Central place that guarantees "alive => opaque and clickable". Called on every
    // live refresh so a leftover FadeOutOnDeath can't bleed into the next room.
    private void ResetVisibilityInvariants(bool force = false)
    {
        if (_dead && !force) return;
        _dead = false;
        _fadeTween?.Kill();
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 1f);
        SetHitboxesEnabled(true);
    }

    private void SetHitboxesEnabled(bool on)
    {
        foreach (var hb in _hitboxes)
            if (hb != null && IsInstanceValid(hb))
                hb.MouseFilter = on ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
    }

    // ── Tracking ────────────────────────────────────────────────────────────────

    public void Track(NCreature creatureNode, Control vfxContainer)
    {
        _creatureNode = creatureNode;
        _vfxContainer = vfxContainer;
        _ungatedProcessTime = 0;
        
        _hexaCenter = creatureNode.FindChild("HexaghostScene", true, false) as Node2D;
        if (_hexaCenter == null)
            HexaghostMainFile.Logger.Warn(
                $"[Ghostflames #{Id}] HexaghostScene not found; falling back to creature-origin offset");

        DownfallControllerNav.LinkAbove(_reachableHitboxes, creatureNode.Hitbox);
    }

    // ── Fades ─────────────────────────────────────────────────────────────────

    public void FadeOutOnDeath(float duration = 0.4f)
    {
        _dead = true;
        _fadeTween?.Kill();
        SetHitboxesEnabled(false);

        _fadeTween = CreateTween();
        _fadeTween.TweenProperty(this, "modulate:a", 0f, duration)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);
    }

    public void FadeInOnRevive(float duration = 0.4f)
    {
        _fadeTween?.Kill();

        _fadeTween = CreateTween();
        _fadeTween.TweenProperty(this, "modulate:a", 1f, duration)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.In)
            .Finished += () =>
            {
                _dead = false;
                SetHitboxesEnabled(true);
            };
    }

    // ── Per-frame ───────────────────────────────────────────────────────────────

    public override void _Process(double delta)
    {
        if (_creatureNode == null || _vfxContainer == null)
        {
            // If we sit here too long it means Track() was never called for this room.
            _ungatedProcessTime += delta;
            if (_ungatedProcessTime is > 1.0 and < 1.0 + 0.05) // log once, ~1s in
                HexaghostMainFile.Logger.Warn(
                    $"[Ghostflames #{Id}] _Process idle >1s: Track() not called? " +
                    $"creature={_creatureNode != null}, vfx={_vfxContainer != null}");
            return;
        }
        _ungatedProcessTime = 0;

        if (!IsInstanceValid(_creatureNode))
        {
            HexaghostMainFile.Logger.Error($"[Ghostflames #{Id}] creature node not valid!");
            return;
        }

        var ct = _creatureNode.GetGlobalTransform();
        var containerScale = _vfxContainer.GetGlobalTransform().Scale;
        var scaleX = GhostflameLayout.ExtraScale(ct.Scale.X, containerScale.X, _creatureNode._tempScale);
        var scaleY = GhostflameLayout.ExtraScale(ct.Scale.Y, containerScale.Y, _creatureNode._tempScale);
        Scale = new Vector2(scaleX, scaleY);

        // Track may have run before the visuals subtree finished entering the tree,
        // so lazily retry the lookup while it's unresolved.
        if (_hexaCenter == null || !IsInstanceValid(_hexaCenter))
            _hexaCenter = _creatureNode.FindChild("HexaghostScene", true, false) as Node2D;

        var globalCenter = _hexaCenter != null && IsInstanceValid(_hexaCenter)
            ? _hexaCenter.GlobalPosition
            : GhostflameLayout.FallbackCenter(_creatureNode.GlobalPosition, scaleY);

        Position = _vfxContainer.GetGlobalTransform().AffineInverse() * globalCenter;

        if (!_loggedTrackState)
        {
            _loggedTrackState = true;
        }

        var intentOffset = GhostflameLayout.IntentOffset(scaleX, scaleY);
        for (var i = 0; i < _fires.Length; i++)
        {
            var fire = _fires[i];
            if (fire == null || !IsInstanceValid(fire)) continue;

            var intent = _intents[i];
            if (intent != null && IsInstanceValid(intent))
            {
                intent.GlobalPosition = fire.GlobalPosition + intentOffset;
                intent.Rotation = -Rotation;
            }

            var anchor = _hitboxAnchors[i];
            if (anchor != null && IsInstanceValid(anchor))
            {
                anchor.GlobalPosition = fire.GlobalPosition;
                // Counter-rotate: the anchor rides this Control (which spins to bring the
                // active flame to the top), so this keeps the hitbox/reticle flat.
                anchor.Rotation = -Rotation;
            }
        }
    }

    // ── Wheel state ─────────────────────────────────────────────────────────────

    private void SetFirePosition(int fireIndex, float duration = 0.5f)
    {
        if (_fires.Length == 0) return;

        _positionTween?.Kill();
        var newRot = GhostflameLayout.ShortestRotationTo(
            Rotation, GhostflameLayout.WheelRotation(fireIndex, _fires.Length));

        _positionTween = CreateTween().SetParallel();
        _positionTween.TweenProperty(this, "rotation", newRot, duration)
            .SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);

        foreach (var fire in _fires)
            if (fire != null && IsInstanceValid(fire))
                _positionTween.TweenProperty(fire, "rotation", -newRot, duration)
                    .SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);

        foreach (var intent in _intents)
            if (intent != null && IsInstanceValid(intent))
                _positionTween.TweenProperty(intent, "rotation", -newRot, duration)
                    .SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
    }

    public void RefreshWheel(GhostflameModel[] wheel, int currentIndex)
    {
        if (_player == null)
        {
            HexaghostMainFile.Logger.Error($"[Ghostflames #{Id}] RefreshWheel with null player");
            return;
        }

        // Self-heal: build (or rebuild) to match the wheel we were actually handed.
        EnsureBuilt(wheel.Length);
        if (_fires.Length == 0)
        {
            HexaghostMainFile.Logger.Error(
                $"[Ghostflames #{Id}] RefreshWheel: still no flames after EnsureBuilt({wheel.Length})");
            return;
        }

        // Alive refresh — make sure nothing stale is hiding us.
        ResetVisibilityInvariants();

        _currentWheel = wheel;

        for (var i = 0; i < Math.Min(wheel.Length, _fires.Length); i++)
        {
            wheel[i].UpdateVisuals();
            _fires[i]?.SetState(
                wheel[i].FireColor,
                wheel[i].IsIgnited ? NFire.FireSize.Large : NFire.FireSize.Small);

            if (_intents[i] != null)
                _intents[i]!.UpdateIntent(wheel[i].Intent, [], _player.Creature);
        }

        _intentTween?.Kill();
        _intentTween = CreateTween().SetParallel();
        for (var i = 0; i < _intents.Length; i++)
        {
            if (_intents[i] == null) continue;
            _intents[i]!.Visible = true;
            _intentTween.TweenProperty(_intents[i], "modulate:a",
                    GhostflameLayout.IntentAlpha(i, currentIndex), 0.3f)
                .SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
        }

        SetFirePosition(currentIndex);

        // "Up" from the creature should land on the active/top flame, which changes
        // independently of screen rotation as the player ignites/rotates the wheel.
        if (_creatureNode != null && IsInstanceValid(_creatureNode))
            DownfallControllerNav.LinkAbove(_reachableHitboxes, _creatureNode.Hitbox, currentIndex);
    }

    public void RefreshCurrentIntent(GhostflameModel[] wheel, int currentIndex, Player player)
    {
        if (currentIndex < 0 || currentIndex >= _intents.Length || currentIndex >= wheel.Length) return;
        _intents[currentIndex]?.UpdateIntent(wheel[currentIndex].Intent, [], player.Creature);
    }

    public Vector2 GetFlameWorldPosition(int index)
    {
        if (index < 0 || index >= _fires.Length) return GlobalPosition;
        var fire = _fires[index];
        return fire != null && IsInstanceValid(fire) ? fire.GlobalPosition : GlobalPosition;
    }
}