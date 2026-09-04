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

public class CollectorMiasmaPower() : CollectorPowerModel(PowerType.Debuff)
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
        //Taken from PoisonPower, skips if owner is not in side taking turn.
        if (!participants.Contains<Creature>(this.Owner))
        {
            return;
        }
        await this.Trigger(choiceContext);
        
        /*
        if (side != Owner.Side || Owner.CombatState == null) return;
        
        var ctx = new BlockingPlayerChoiceContext();
        
        var damage = CollectorHook.ModifyCollectorMiasmaIncrement(Owner.CombatState, Owner, Amount);
        
        var results = await CreatureCmd.Damage(ctx, Owner, damage,
            DamageProps.nonCardHpLoss, null, null);

        if (results.Any(r => r.WasTargetKilled)) SfxCmd.Play("event:/sfx/ui/relics/relic_prayer_bowl", 3);
        
        if (TestMode.IsOff && results.Any(r => r.WasTargetKilled)) SfxCmd.Play("event:/sfx/ui/relics/relic_prayer_bowl", 3);


        if (Owner.IsAlive)
        {
            if (!Owner.IsAfflicted && !CollectorHook.PreventDoomRemoval(Owner.CombatState, Owner))
                await PowerCmd.Remove(this);
        }
        
        else
        {
            await Cmd.CustomScaledWait(0.1f, 0.25f);
        }
        */
    }

    public async Task Trigger(PlayerChoiceContext ctx)
    {
        int stacks = Owner.GetPowerAmount<DemisePower>();//The amount of demise stacks the target has.
        int? extraStacks = CollectorHook.ModifyCollectorMiasmaIncrement(Owner.CombatState!, Owner, Amount);//Jade Ring modifier for incrementing.
        if (extraStacks > 0)
        {
            stacks += (int)extraStacks;
        }

        IEnumerable<DamageResult> damageResults = await CreatureCmd.Damage(
            new ThrowingPlayerChoiceContext(), Owner, Amount,
            DamageProps.nonCardUnpowered, null, null);
        
        //If demise is present apply stacks.
        if (Owner.IsAlive && stacks > 0)
            await PowerCmd.Apply<CollectorMiasmaPower>(ctx, this.Owner, stacks, this.Owner, null);
        else
            await Cmd.CustomScaledWait(0.1f, 0.25f);
    }
    
}