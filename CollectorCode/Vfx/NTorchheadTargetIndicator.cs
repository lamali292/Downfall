using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Powers;
using Godot;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Collector.CollectorCode.Vfx;

/// <summary>
///     Crosshair pinned to an enemy's health bar while the local player's Torchhead would attack that enemy. It is
///     dimmed while Torchhead would not attack at the end of this turn (nothing was hit yet), and only shown when
///     there is more than one enemy, since a lone enemy needs no marking.
///     Sizing lives in torchhead_target_indicator.tscn; this script decides when it is shown and where it sits.
/// </summary>
public partial class NTorchheadTargetIndicator : TextureRect
{
    // Horizontal offset from the bar's right edge to the crosshair's left edge; negative pulls it back over the bar.
    private const float HorizontalGap = -30f;

    private const float PulseFrequency = 2f;
    private const float PulseScaleMax = 1.1f;
    private const float PulseOpacityMin = 0.7f;
    private const float PulseOpacityMax = 0.9f;
    private const float IdleOpacityFactor = 0.5f;

    private NCreature _creature = null!;
    private Control _anchor = null!;
    private float _time;

    public static NTorchheadTargetIndicator Create(NCreature creature, Control anchor)
    {
        var indicator = ResourceLoader
            .Load<PackedScene>("res://Collector/scenes/ui/torchhead_target_indicator.tscn")
            .Instantiate<NTorchheadTargetIndicator>();
        indicator._creature = creature;
        indicator._anchor = anchor;
        return indicator;
    }

    // Polled instead of event driven: the target changes through hits, kills, Whomp and Torchhead dying or being
    // kindled, and a missed event would leave a stale marker. It is a handful of lookups per enemy per frame.
    public override void _Process(double delta)
    {
        var show = ShouldShow(out var willAttack);
        if (show != Visible)
        {
            Visible = show;
            if (!show) return;
            _time = 0f;
            UpdatePosition();
        }

        if (!show) return;
        _time += (float)delta * PulseFrequency;
        var t = (Mathf.Sin(_time) + 1f) * 0.5f;
        var opacity = Mathf.Lerp(PulseOpacityMin, PulseOpacityMax, t) * (willAttack ? 1f : IdleOpacityFactor);
        Scale = Vector2.One * Mathf.Lerp(1f, PulseScaleMax, t);
        Modulate = new Color(1, 1, 1, opacity);
    }

    private bool ShouldShow(out bool willAttack)
    {
        willAttack = false;
        var combatState = _creature.Entity.CombatState;
        if (combatState == null || combatState.HittableEnemies.Count() <= 1) return false;

        var player = LocalContext.GetMe(combatState);
        if (player == null || !player.IsTorchheadAlive) return false;

        var power = player.Torchhead?.GetPower<TorchheadPower>();
        if (power == null || power.AttackTarget != _creature.Entity) return false;

        willAttack = power.ShouldAutoAttack;
        return true;
    }

    /// <summary>
    ///     Places the crosshair from the anchor's screen rect, vertically centred on the bar. The scene puts the pivot
    ///     at the crosshair's centre, so the pulse scales in place. Assigning GlobalPosition keeps it independent of
    ///     where the node sits in the tree.
    /// </summary>
    private void UpdatePosition()
    {
        var rect = _anchor.GetGlobalRect();
        GlobalPosition = new Vector2(
            rect.Position.X + rect.Size.X + HorizontalGap,
            rect.Position.Y + rect.Size.Y * 0.5f - Size.Y * 0.5f);
    }
}
