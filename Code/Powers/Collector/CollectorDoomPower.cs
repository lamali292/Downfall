using BaseLib.Hooks;
using Downfall.Code.Abstract;
using Downfall.Code.Events;
using Downfall.Code.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Powers.Collector;

public class CollectorDoomPower() : CollectorPowerModel(PowerType.Debuff)
{
    public override IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext ctx)
    {
        if (Amount <= 0) yield break;

        yield return new HealthBarForecastSegment(
            Amount,
            new Color("880088"),
            HealthBarForecastDirection.FromRight,
            0
        );
    }

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Side || Owner.CombatState == null) return;

        var damage = DownfallHook.ModifyCollectorDoomDamage(Owner.CombatState, Owner, Amount);
        var results = await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner, damage,
            ValueProp.Unblockable | ValueProp.Unpowered, null, null);

        if (results.Any(r => r.WasTargetKilled)) SfxCmd.Play("event:/sfx/ui/relics/relic_prayer_bowl", 3);


        if (Owner.IsAlive)
        {
            if (!Owner.IsAfflicted() && !DownfallHook.PreventDoomRemoval(Owner.CombatState, Owner))
                await PowerCmd.Remove(this);
        }
        else
        {
            await Cmd.CustomScaledWait(0.1f, 0.25f);
        }
    }
}