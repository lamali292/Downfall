using Godot;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Hexaghost.HexaghostCode.Vfx;

public partial class NGhostflames : Control
{
    private NFire? _fire1;
    private NFire? _fire2;
    private NFire? _fire3;
    private NFire? _fire4;
    private NFire? _fire5;
    private NFire? _fire6;
    private Node2D?[] _hitboxAnchors = [];
    private NIntent?[] _intents = [];
    private NFire?[] AllFires => [_fire1, _fire2, _fire3, _fire4, _fire5, _fire6];
    private Tween? _intentTween;
    private Tween? _positionTween;
    private Player? _player;
    private GhostflameModel[]? _currentWheel;

    private NCreature? _creatureNode;
    private Control? _vfxContainer;
    private bool _loggedTrackState;

    public override void _Ready()
    {
        _fire1 = GetNode<NFire>("%fire1");
        _fire2 = GetNode<NFire>("%fire2");
        _fire3 = GetNode<NFire>("%fire3");
        _fire4 = GetNode<NFire>("%fire4");
        _fire5 = GetNode<NFire>("%fire5");
        _fire6 = GetNode<NFire>("%fire6");

        _intents = AllFires.Select((fire, i) =>
        {
            if (fire == null) return null;
            var intent = NIntent.Create(i * 0.3f);
            intent.Visible = false;
            intent.MouseFilter = Control.MouseFilterEnum.Ignore;
            AddChild(intent);
            return intent;
        }).ToArray();

        _hitboxAnchors = AllFires.Select((fire, i) =>
        {
            if (fire == null) return null;
            var anchor = new Node2D();
            AddChild(anchor);

            var hitbox = new Control();
            hitbox.CustomMinimumSize = new Vector2(80, 80);
            hitbox.Position = -hitbox.CustomMinimumSize / 2f;
            hitbox.MouseFilter = MouseFilterEnum.Stop;
            anchor.AddChild(hitbox);

            hitbox.Connect(Control.SignalName.MouseEntered, Callable.From(() =>
            {
                var flame = _currentWheel?.ElementAtOrDefault(i);
                if (flame == null) return;
                NCombatRoom.Instance?.GetCreatureNode(_player!.Creature)
                    ?.ShowHoverTips([flame.HoverTip]);
            }));
            hitbox.Connect(Control.SignalName.MouseExited,
                Callable.From(() => { NCombatRoom.Instance?.GetCreatureNode(_player!.Creature)?.HideHoverTips(); }));
            return anchor;
        }).ToArray();
    }

    public void Track(NCreature creatureNode, Control vfxContainer)
    {
        _creatureNode = creatureNode;
        _vfxContainer = vfxContainer;
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
            GD.Print($"[Ghostflames] tracking={_creatureNode != null && IsInstanceValid(_creatureNode)}");
        }

        for (var i = 0; i < _intents.Length; i++)
        {
            var fire = AllFires[i];
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
            if (_hitboxAnchors[i] != null)
                _hitboxAnchors[i]!.GlobalPosition = fire.GlobalPosition;
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

        foreach (var fire in AllFires)
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
        for (var i = 0; i < wheel.Length; i++)
        {
            AllFires[i]?.SetState(wheel[i].FireColor, wheel[i].IsIgnited ? NFire.FireSize.Large : NFire.FireSize.Small);
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
    }

    public void RefreshCurrentIntent(GhostflameModel[] wheel, int currentIndex, Player player)
    {
        _intents[currentIndex]!.UpdateIntent(wheel[currentIndex].Intent, [], player.Creature);
    }

    public Vector2 GetFlameWorldPosition(int index)
    {
        var fire = AllFires[index];
        return fire?.GlobalPosition ?? GlobalPosition;
    }
}