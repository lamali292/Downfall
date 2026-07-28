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
    private static readonly Vector2 ReticleVisualSize = new(44, 44);

    // Fire sprites are positioned with their origin at the base, not their visual center —
    // without this the bracket reads as centered too low, well below the flame itself.
    private static readonly Vector2 ReticleCenterOffset = new(0, -22);

    private NCreature? _creatureNode;
    private GhostflameModel[]? _currentWheel;
    private NFire? _fire1;
    private NFire? _fire2;
    private NFire? _fire3;
    private NFire? _fire4;
    private NFire? _fire5;
    private NFire? _fire6;
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

    public override void _Ready()
    {
        _fire1 = GetNode<NFire>("%fire1");
        _fire2 = GetNode<NFire>("%fire2");
        _fire3 = GetNode<NFire>("%fire3");
        _fire4 = GetNode<NFire>("%fire4");
        _fire5 = GetNode<NFire>("%fire5");
        _fire6 = GetNode<NFire>("%fire6");
        _allFires = [_fire1, _fire2, _fire3, _fire4, _fire5, _fire6];

        // Pre-size and index-assign (rather than .Select(...).ToArray()) so a throw partway
        // through — e.g. AttachFocusReticle hitting an unloaded scene — can't leave these
        // arrays at length 0 while _Process still iterates 6 slots. That mismatch was the
        // original per-frame IndexOutOfRangeException storm.
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

            // Sibling of the hitbox, centered on the same anchor origin (matches how
            // orb.tscn positions its own SelectionReticle relative to the orb's hitbox).
            // Sized to the flame sprite itself, not the (deliberately oversized, for easier
            // targeting) 80x80 hitbox — otherwise the bracket reads as loose/oversized.
            _reticles[i] = DownfallControllerNav.AttachFocusReticle(anchor, ReticleCenterOffset, ReticleVisualSize, 4f);
            _hitboxAnchors[i] = anchor;
        }

        _reachableHitboxes = _hitboxes.Where(h => h != null).Cast<Control>().ToList();

        // Each hitbox gets its own hover callback keyed to its wheel index (not the
        // wheel's rotation, which only ever changes fire/anchor positions, never identity).
        for (var i = 0; i < _hitboxes.Length; i++)
        {
            var hitbox = _hitboxes[i];
            if (hitbox == null) continue;
            var index = i;
            var reticle = _reticles[i];
            DownfallControllerNav.WireHover(hitbox,
                () =>
                {
                    // Matches NOrb.OnFocus: the reticle is a controller-only affordance —
                    // mouse hover should still show the tooltip but never draw the bracket.
                    if (NControllerManager.Instance?.IsUsingController == true) reticle?.OnSelect();
                    var flame = _currentWheel?.ElementAtOrDefault(index);
                    if (flame == null) return;
                    NCombatRoom.Instance?.GetCreatureNode(_player!.Creature)?.ShowHoverTips([flame.HoverTip]);
                },
                () =>
                {
                    reticle?.OnDeselect();
                    NCombatRoom.Instance?.GetCreatureNode(_player!.Creature)?.HideHoverTips();
                });
        }

        // Ring topology: the wheel wraps around, so left/right should too.
        DownfallControllerNav.WireChain(_reachableHitboxes, true);
    }

    public void Track(NCreature creatureNode, Control vfxContainer)
    {
        _creatureNode = creatureNode;
        _vfxContainer = vfxContainer;

        DownfallControllerNav.LinkAbove(_reachableHitboxes, creatureNode.Hitbox);
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
            var globalCenter = _creatureNode.GlobalPosition + Vector2.Up * 216f * scaleY;
            Position = _vfxContainer.GetGlobalTransform().AffineInverse() * globalCenter;
        }

        if (!_loggedTrackState)
        {
            _loggedTrackState = true;
            HexaghostMainFile.Logger.Info($"[Ghostflames] tracking={_creatureNode != null && IsInstanceValid(_creatureNode)}");
        }

        for (var i = 0; i < _allFires.Length; i++)
        {
            var fire = _allFires[i];
            if (fire == null) continue;

            var worldPos = fire.GlobalPosition
                           + Vector2.Up * 130f * scaleY
                           + Vector2.Left * 25f * scaleX;

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
        _positionTween?.Kill();
        var targetRot = -(fireIndex - 0.5) * Mathf.Tau / 6f;
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

    public void RefreshWheel(GhostflameModel[] wheel, int currentIndex, Player player)
    {
        _currentWheel = wheel;
        _player = player;
        for (var i = 0; i < Math.Min(wheel.Length, _allFires.Length); i++)
        {
            _allFires[i]?.SetState(wheel[i].FireColor, wheel[i].IsIgnited ? NFire.FireSize.Large : NFire.FireSize.Small);
            if (_intents[i] == null) continue;
            _intents[i]!.UpdateIntent(wheel[i].Intent, [], player.Creature);
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