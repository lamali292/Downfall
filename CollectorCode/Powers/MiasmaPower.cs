using BaseLib.Hooks;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Events;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Powers;

public class MiasmaPower() : CollectorPowerModel(PowerType.Debuff)
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

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner)) return;
        await Trigger(choiceContext);
        
    }

    public async Task Trigger(PlayerChoiceContext ctx)
    {
        //The amount of demise stacks the target has.
        var stacks = Owner.GetPowerAmount<MegaCrit.Sts2.Core.Models.Powers.DemisePower>();
        //Jade Ring modifier for incrementing.
        var extraStacks = CollectorHook.ModifyCollectorMiasmaIncrement(Owner.CombatState!, Owner, 0);
        if (extraStacks > 0) stacks += extraStacks;

        await CreatureCmd.Damage(
            new BlockingPlayerChoiceContext(), Owner, Amount,
            DamageProps.nonCardUnpowered, null, null);
        
        //If demise is present apply stacks.
        if (Owner.IsAlive && stacks > 0)
            await PowerCmd.Apply<MiasmaPower>(ctx, Owner, stacks, Owner, null);
    }
    
}