using Downfall.Code.Core.Guardian;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;

namespace Downfall.Code.Vfx.Guardian;

public partial class NGemShootEffect : Node2D
{
    private Vector2 _from;

    private GemModel? _gem;
    private int _hitNo;
    private Sprite2D? _sprite;
    private Vector2 _target;
    private int _total;

    public static NGemShootEffect Create(GemModel gem, int hitNo, Vector2 from, Vector2 target, int total)
    {
        var effect = new NGemShootEffect();
        effect._gem = gem;
        effect._from = from;
        effect._target = target;
        effect._hitNo = hitNo;
        effect._total = total;
        return effect;
    }

    public override void _Ready()
    {
        if (_gem == null) return;
        _sprite = new Sprite2D();
        _sprite.Texture = PreloadManager.Cache.GetAsset<Texture2D>(_gem.IconPath);
        _sprite.Scale = new Vector2(0.5f, 0.5f);
        AddChild(_sprite);

        GlobalPosition = _from;

        var spread = new Vector2(
            _from.X + (float)GD.RandRange(-200f, 200f),
            _from.Y + (float)GD.RandRange(-200f, 200f)
        );

        var delay = _hitNo * 0.2f;
        var tween = CreateTween();

        tween.TweenProperty(this, "global_position", spread, 0.3f)
            .SetDelay(delay)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(this, "global_position", _target, 0.3f)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.In);
        var rotateTween = CreateTween();
        rotateTween.TweenProperty(_sprite, "rotation", Mathf.Tau * 3f, 0.6f + delay)
            .SetDelay(delay);
        tween.Parallel().TweenProperty(_sprite, "scale", new Vector2(0.7f, 0.35f), 0.3f);
        tween.TweenCallback(Callable.From(PlayHitSound));
        tween.TweenCallback(Callable.From(QueueFree));
    }

    private void PlayHitSound()
    {
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/cultists/cultists_attack");
    }
}