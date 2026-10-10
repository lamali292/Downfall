using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Nodes.Combat;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Vfx;

/// <summary>
/// Small badge in the bottom-right corner of a slime showing what its next Command will do
/// (<see cref="SlimeModel.CounterValue"/>: damage, or the secondary value for block/thorns slimes),
/// including Potency and other modifiers. Green when above the slime's base value, red when below.
/// </summary>
/// <remarks>
/// The value is re-read a few times a second, but the label is only touched (and popped) when it actually
/// changed: it depends on arbitrary damage modifiers (powers, relics, other mods) and combat history,
/// which have no single change event to subscribe to.
/// </remarks>
public partial class NSlimeCounter : Control
{
    private const string ScenePath = "ui/slime_counter.tscn";
    private const float RefreshSeconds = 0.2f;
    private static readonly Color Higher = new(0.5f, 1f, 0.5f);
    private static readonly Color Lower = new(1f, 0.45f, 0.45f);

    private NCreature _creatureNode = null!;
    private SlimeModel _slime = null!;
    private TextureRect _icon = null!;
    private MegaLabel _value = null!;
    private int? _shownValue;
    private Tween? _popTween;
    private float _sinceRefresh;

    public static NSlimeCounter Create(NCreature creatureNode, SlimeModel slime)
    {
        var counter = GD.Load<PackedScene>(ScenePath.SlimeScenePath()).Instantiate<NSlimeCounter>();
        counter._creatureNode = creatureNode;
        counter._slime = slime;
        return counter;
    }

    public override void _Ready()
    {
        _icon = GetNode<TextureRect>("%Icon");
        _value = GetNode<MegaLabel>("%Value");
        _icon.Texture = GD.Load<Texture2D>(_slime.CounterIconPath);
        ZIndex = 2;
        Refresh();
    }

    public override void _Process(double delta)
    {
        _sinceRefresh += (float)delta;
        if (_sinceRefresh < RefreshSeconds) return;
        _sinceRefresh = 0f;
        Refresh();
    }

    private void Refresh()
    {
        var shown = _slime.Creature.IsAlive ? _slime.CounterValue : null;
        Visible = shown != null;
        if (shown is not { } value) return;

        var baseValue = _slime.CounterBaseValue;
        _value.Modulate = value > baseValue ? Higher : value < baseValue ? Lower : Colors.White;
        if ((int)value != _shownValue)
        {
            var isFirst = _shownValue == null;
            _shownValue = (int)value;
            _value.SetTextAutoSize(_shownValue.ToString()!);
            if (!isFirst) Pop();
        }

        var hitbox = _creatureNode.Hitbox;
        GlobalPosition = hitbox.GlobalPosition + hitbox.Size - Size;
    }

    /// <summary>Brief scale-up so a changed value (e.g. the Cultist Slime growing) catches the eye.</summary>
    private void Pop()
    {
        _popTween?.Kill();
        _popTween = CreateTween();
        _popTween.TweenProperty(this, "scale", Vector2.One, 0.3).From(Vector2.One * 1.5f)
            .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
    }
}
