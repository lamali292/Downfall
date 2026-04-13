using BaseLib.Hooks;
using Downfall.Code.Abstract;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Powers.Collector;

public class CollectorDoomPower() : CollectorPowerModel(PowerType.Debuff)
{
    public override IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext ctx)
    {
        if (Amount <= 0) yield break;
        
        yield return new HealthBarForecastSegment(
            amount:    Amount,
            color:     new Color("880088"),
            direction: HealthBarForecastDirection.FromLeft,
            order:     0
        );
    }
    
    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Side) return;

        var results = await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner, Amount, ValueProp.Unblockable | ValueProp.Unpowered, null, null);

        if (results.Any(r => r.WasTargetKilled))
        {
            SfxCmd.Play("event:/sfx/ui/relics/relic_prayer_bowl", 3);
        }
            
        
        if (Owner.IsAlive)
        {
            if (!IsAfflicted(Owner))
                await PowerCmd.Remove(this);
        }
        else
            await Cmd.CustomScaledWait(0.1f, 0.25f);
    }
    

    public static bool IsAfflicted(Creature creature)
    {
        return creature.GetPowerAmount<VulnerablePower>() > 0 && creature.GetPowerAmount<WeakPower>() > 0;
    }
    
}