using BaseLib.Utils;
using Downfall.DownfallCode.Utils.UI;
using Godot;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Hexaghost.HexaghostCode.Vfx;

public partial class NGhostflames : Control
{
    private static string FireScenePath => "res://Hexaghost/scenes/character/hexaghost_flame.tscn";

    private static Vector2 FirePositionFor(int index, int count)
    {
        var angle = Mathf.Pi / 2f + Mathf.Pi / count - index * Mathf.Tau / count;
        return new Vector2(
            WheelRadius * Mathf.Cos(angle),
            -WheelRadius * Mathf.Sin(angle)); // screen y is down
    }

    private const float WheelRadius = 140f;
    private static readonly Vector2 FireScale = new(1, 1);

    // Sized at Create() time from the wheel, not fixed at 6.
    private NFire?[] _builtFires = [];

    private static readonly Vector2 ReticleVisualSize = new(44, 44);

    // Fire sprites are positioned with their origin at the base, not their visual center —
    // without this the bracket reads as centered too low, well below the flame itself.
    private static readonly Vector2 ReticleCenterOffset = new(0, -22);

    private NCreature? _creatureNode;
    private Node2D? _hexaCenter; // %HexaghostScene inside the creature visuals — the ring's center
    private GhostflameModel[]? _currentWheel;
    private Node2D?[] _hitboxAnchors = [];
    private Control?[] _hitboxes = [];
    private NIntent?[] _intents = [];
    private Tween? _intentTween;
    private bool _loggedTrackState;
    private Player? _player;
    private Tween? _positionTween;
    private List<Control> _reachableHitboxes = [];
    private NSelectionReticle?[] _reticles = [];
    private Control? _vfxContainer;
    private NFire?[] _allFires = [];

    public static NGhostflames Create(Player player)
    {
        var root = new NGhostflames { Name = "Ghostflames" };
        root._player = player;
        root.ZIndex = 0;
        root.ZAsRelative = false;
        var fireScene = ResourceLoader.Load<PackedScene>(FireScenePath);
        if (fireScene == null)
        {
            HexaghostMainFile.Logger.Error($"[Ghostflames] failed to load {FireScenePath}");
            return root;
        }
        var count = HexaghostModel.Wheel.Get(player)?.Length ?? 0;
        
        var fires = new NFire?[count];
        for (var i = 0; i < count; i++)
        {
            var fire = fireScene.Instantiate<NFire>();
            fire.Name = $"fire{i + 1}";
            fire.Position = FirePositionFor(i, count);
            fire.Scale = FireScale;
            root.AddChild(fire);
            fires[i] = fire;
        }

        root._builtFires = fires;

        return root;
    }

    public override void _Ready()
    {
        _allFires = _builtFires;
        _intents = new NIntent?[_allFires.Length];
        _hitboxes = new Control?[_allFires.Length];
        _reticles = new NSelectionReticle?[_allFires.Length];
        _hitboxAnchors = new Node2D?[_allFires.Length];

        for (var i = 0; i < _allFires.Length; i++)
        {
            if (_allFires[i] == null) continue;

            var intent = NIntent.Create(i * 0.3f);
            intent.Visible = false;
            intent.MouseFilter = MouseFilterEnum.Ignore;
            AddChild(intent);
            _intents[i] = intent;

            var anchor = new Node2D();
            AddChild(anchor);

            var hitbox = new Control
            {
                CustomMinimumSize = new Vector2(80, 80),
                MouseFilter = MouseFilterEnum.Stop
            };
            hitbox.Position = -hitbox.CustomMinimumSize / 2f;
            anchor.AddChild(hitbox);
            _hitboxes[i] = hitbox;

            // A mouse click grabs focus on this hitbox; because WireHover latches on
            // (isHovered || isFocused), that lingering focus both keeps the tip up after
            // the mouse leaves and blocks onFocus from re-firing when it returns. Drop
            // focus on mouse-up so the hover tip is driven purely by mouse enter/exit.
            // Controller focus (d-pad/stick) is unaffected.
            var hb = hitbox;
            hb.GuiInput += (InputEvent ev) =>
            {
                if (ev is InputEventMouseButton { Pressed: false })
                    hb.ReleaseFocus();
            };

            _reticles[i] = DownfallControllerNav.AttachFocusReticle(anchor, ReticleCenterOffset, ReticleVisualSize, 4f);
            _hitboxAnchors[i] = anchor;
        }

        _reachableHitboxes = _hitboxes.Where(h => h != null).Cast<Control>().ToList();

        for (var i = 0; i < _hitboxes.Length; i++)
        {
            var hitbox = _hitboxes[i];
            if (hitbox == null) continue;
            var index = i;
            var reticle = _reticles[i];
            DownfallControllerNav.WireHover(hitbox,
                () =>
                {
                    if (NControllerManager.Instance?.IsUsingButtonInputsCompatibility() == true) reticle?.OnSelect();
                    var flame = _currentWheel?.ElementAtOrDefault(index);
                    if (flame == null) return;
                    NCombatRoom.Instance?.GetCreatureNode(_player!.Creature)?.ShowHoverTips(flame.HoverTips);
                },
                () =>
                {
                    reticle?.OnDeselect();
                    NCombatRoom.Instance?.GetCreatureNode(_player!.Creature)?.HideHoverTips();
                });
        }

        DownfallControllerNav.WireChain(_reachableHitboxes, true);
    }

    public void Track(NCreature creatureNode, Control vfxContainer)
    {
        _creatureNode = creatureNode;
        _vfxContainer = vfxContainer;

        // %HexaghostScene is nested inside %Visuals -> Hexaghost (instanced scene),
        // so it isn't owned by the creature node — owned:false lets FindChild descend
        // into that sub-scene. This is the true visual center; anchoring the ring to it
        // keeps the flames riding the creature's idle bob / hurt shake / cast lunge
        // instead of sitting at a fixed offset from the creature root.
        _hexaCenter = creatureNode.FindChild("HexaghostScene", true, false) as Node2D;
        if (_hexaCenter == null)
            HexaghostMainFile.Logger.Warn(
                "[Ghostflames] HexaghostScene not found; falling back to creature-origin offset");

        DownfallControllerNav.LinkAbove(_reachableHitboxes, creatureNode.Hitbox);
    }

    private Tween? _fadeTween;

    public void FadeOutOnDeath(float duration = 0.4f)
    {
        _fadeTween?.Kill();

        SetHitboxesEnabled(false);

        _fadeTween = CreateTween();
        _fadeTween.TweenProperty(this, "modulate:a", 0f, duration)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);
    }

    private void SetHitboxesEnabled(bool on)
    {
        foreach (var hb in _hitboxes)
            if (hb != null)
                hb.MouseFilter = on ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
    }

    public void FadeInOnRevive(float duration = 0.4f)
    {
        _fadeTween?.Kill();

        _fadeTween = CreateTween();
        _fadeTween.TweenProperty(this, "modulate:a", 1f, duration)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.In)
            .Finished += () => SetHitboxesEnabled(true);
    }

    // TODO : make transition more clean for Shrinker Beetle scaling
    public override void _Process(double delta)
    {
        if (_creatureNode == null || _vfxContainer == null) return;
        var ct = _creatureNode.GetGlobalTransform();

        var containerScale = _vfxContainer.GetGlobalTransform().Scale;
        var sx = Mathf.Abs(ct.Scale.X / containerScale.X);
        var sy = Mathf.Abs(ct.Scale.Y / containerScale.Y);
        var scaleX = _creatureNode._tempScale * sx;
        var scaleY = _creatureNode._tempScale * sy;
        if (IsInstanceValid(_creatureNode) && _vfxContainer != null)
        {
            Scale = new Vector2(scaleX, scaleY);

            // Track may have run a frame before the visuals subtree finished entering the
            // tree, so lazily retry the lookup if it's still unresolved.
            if (_hexaCenter == null || !IsInstanceValid(_hexaCenter))
                _hexaCenter = _creatureNode.FindChild("HexaghostScene", true, false) as Node2D;

            var globalCenter = _hexaCenter != null && IsInstanceValid(_hexaCenter)
                ? _hexaCenter.GlobalPosition
                : _creatureNode.GlobalPosition + Vector2.Up * 170f * scaleY;

            Position = _vfxContainer.GetGlobalTransform().AffineInverse() * globalCenter;
        }

        if (!_loggedTrackState)
        {
            _loggedTrackState = true;
            HexaghostMainFile.Logger.Info(
                $"[Ghostflames] tracking={_creatureNode != null && IsInstanceValid(_creatureNode)}");
        }

        for (var i = 0; i < _allFires.Length; i++)
        {
            var fire = _allFires[i];
            if (fire == null) continue;

            var worldPos = fire.GlobalPosition
                           + Vector2.Up * 130f * scaleY
                           + Vector2.Left * 33f * scaleX;

            var intent = _intents[i];
            if (intent != null)
            {
                intent.GlobalPosition = worldPos;
                intent.Rotation = -Rotation;
            }

            var anchor = _hitboxAnchors[i];
            if (anchor != null)
            {
                anchor.GlobalPosition = fire.GlobalPosition;
                // Counter-rotate, same as fire/intent above: the anchor is a child of this
                // Control (which itself spins to bring the active flame to the top), so
                // without this the hitbox + focus reticle riding on it would tilt with the
                // wheel instead of staying flat.
                anchor.Rotation = -Rotation;
            }
        }
    }

    private void SetFirePosition(int fireIndex, float duration = 0.5f)
    {
        if (_allFires.Length == 0) return;

        _positionTween?.Kill();
        var targetRot = -(fireIndex - 0.5) * Mathf.Tau / _allFires.Length;
        var current = Rotation;
        var diff = Mathf.AngleDifference(current, targetRot);
        var newRot = current + diff;

        _positionTween = CreateTween().SetParallel();
        _positionTween.TweenProperty(this, "rotation", newRot, duration)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.InOut);

        foreach (var fire in _allFires)
            if (fire != null)
                _positionTween.TweenProperty(fire, "rotation", -newRot, duration)
                    .SetTrans(Tween.TransitionType.Sine)
                    .SetEase(Tween.EaseType.InOut);

        foreach (var intent in _intents)
            if (intent != null)
                _positionTween.TweenProperty(intent, "rotation", -newRot, duration)
                    .SetTrans(Tween.TransitionType.Sine)
                    .SetEase(Tween.EaseType.InOut);
    }

    public void RefreshWheel(GhostflameModel[] wheel, int currentIndex)
    {
        if (_player == null) return;
        _currentWheel = wheel;
        for (var i = 0; i < Math.Min(wheel.Length, _allFires.Length); i++)
        {
            wheel[i].UpdateVisuals();
            _allFires[i]?.SetState(wheel[i].FireColor,
                wheel[i].IsIgnited ? NFire.FireSize.Large : NFire.FireSize.Small);
            if (_intents[i] == null) continue;
            _intents[i]!.UpdateIntent(wheel[i].Intent, [], _player.Creature);
        }

        _intentTween?.Kill();
        _intentTween = CreateTween().SetParallel();
        for (var i = 0; i < _intents.Length; i++)
        {
            if (_intents[i] == null) continue;
            var targetAlpha = i == currentIndex ? 1f : 0f;
            _intents[i]!.Visible = true;
            _intentTween.TweenProperty(_intents[i], "modulate:a", targetAlpha, 0.3f)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.InOut);
        }

        SetFirePosition(currentIndex);

        // Keep controller-nav "up" from the creature landing on whichever flame is
        // currently active/top, not always index 0 — the wheel's active flame changes
        // independently of screen rotation as the player ignites/rotates it.
        if (_creatureNode != null)
            DownfallControllerNav.LinkAbove(_reachableHitboxes, _creatureNode.Hitbox, currentIndex);
    }

    public void RefreshCurrentIntent(GhostflameModel[] wheel, int currentIndex, Player player)
    {
        if (currentIndex < 0 || currentIndex >= _intents.Length || currentIndex >= wheel.Length) return;
        _intents[currentIndex]?.UpdateIntent(wheel[currentIndex].Intent, [], player.Creature);
    }

    public Vector2 GetFlameWorldPosition(int index)
    {
        if (index < 0 || index >= _allFires.Length) return GlobalPosition;
        var fire = _allFires[index];
        return fire?.GlobalPosition ?? GlobalPosition;
    }
}