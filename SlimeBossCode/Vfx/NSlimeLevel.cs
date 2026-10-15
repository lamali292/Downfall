using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Combat;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Vfx;

/// <summary>
/// "Lv. X" label for slimes that level up (<see cref="SlimeModel.Level"/>). It also grows the slime's model to
/// <see cref="SlimeModel.VisualScale"/>, so the level is visible at a glance. Sits at the slime's bottom-right corner,
/// next to the effect counter (<see cref="NSlimeCounter"/>).
/// </summary>
public partial class NSlimeLevel : Control
{
    private const string ScenePath = "ui/slime_level.tscn";
    private const string LocTable = "monsters";
    private const string LocKey = "SLIMEBOSS-SLIME_LEVEL.label";
    private const float RefreshSeconds = 0.2f;

    /// <summary>Where the label's top-left sits relative to the slime hitbox's bottom-right corner.</summary>
    private static readonly Vector2 CornerOffset = new(-106f, 2f);

    private NCreature _creatureNode = null!;
    private SlimeModel _slime = null!;
    private MegaRichTextLabel _label = null!;
    private int? _shownLevel;
    private Tween? _growTween;
    private float _sinceRefresh;

    public static NSlimeLevel Create(NCreature creatureNode, SlimeModel slime)
    {
        var level = GD.Load<PackedScene>(ScenePath.SlimeScenePath()).Instantiate<NSlimeLevel>();
        level._creatureNode = creatureNode;
        level._slime = slime;
        return level;
    }

    public override void _Ready()
    {
        _label = GetNode<MegaRichTextLabel>("%Level");
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
        var level = _slime.Creature.IsAlive ? _slime.Level : null;
        Visible = level != null;
        PlaceInCorner();
        if (level == _shownLevel || level is not { } value) return;

        var isFirst = _shownLevel == null;
        _shownLevel = level;
        var text = new LocString(LocTable, LocKey);
        text.Add("Level", value);
        _label.Text = text.GetFormattedText();

        var scale = Vector2.One * _slime.VisualScale;
        _growTween?.Kill();
        if (isFirst)
        {
            _creatureNode.Visuals.Scale = scale;
            return;
        }

        _growTween = CreateTween();
        _growTween.TweenProperty(_creatureNode.Visuals, "scale", scale, 0.4)
            .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
    }

    private void PlaceInCorner()
    {
        var hitbox = _creatureNode.Hitbox;
        GlobalPosition = hitbox.GlobalPosition + hitbox.Size + CornerOffset;
    }
}
