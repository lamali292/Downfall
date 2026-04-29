using BaseLib.Hooks;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Powers;

public class CollectorDoomPower() : CollectorPowerModel(PowerType.Debuff)
{
    public override IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext ctx)
    {
        if (Amount <= 0) yield break;

        yield return new HealthBarForecastSegment(
            Amount,
            new Color("880088"),
            HealthBarForecastDirection.FromRight
        );
    }

    protected override async Task AfterSideTurnStart(PlayerChoiceContext ctx, CombatSide side, ICombatState combatState)
    {
        if (side != Owner.Side || Owner.CombatState == null) return;

        var damage = CollectorHook.ModifyCollectorDoomDamage(Owner.CombatState, Owner, Amount);
        var results = await CreatureCmd.Damage(ctx, Owner, damage,
            ValueProp.Unblockable | ValueProp.Unpowered, null, null);

        if (results.Any(r => r.WasTargetKilled)) SfxCmd.Play("event:/sfx/ui/relics/relic_prayer_bowl", 3);


        if (Owner.IsAlive)
        {
            if (!Owner.IsAfflicted() && !CollectorHook.PreventDoomRemoval(Owner.CombatState, Owner))
                await PowerCmd.Remove(this);
        }
        else
        {
            await Cmd.CustomScaledWait(0.1f, 0.25f);
        }
    }
}