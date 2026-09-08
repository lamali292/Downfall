using BaseLib.Extensions;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Powers;

public class WaterfallGiantCardPower : CollectorPowerModel
{
    
    public WaterfallGiantCardPower()
    {
        WithPower<MiasmaPower>(0);
    }
    
    
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    
    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext ctx,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
            return;
        if (Amount > 1)
        {
            await PowerCmd.Decrement(this);
        }
        else
        {
            Flash();
            await MyCommonActions.Apply<MiasmaPower>(ctx, this, CombatState.HittableEnemies);
            await PowerCmd.Remove(this);
        }
    }
    
    public void SetMiasma(decimal baseValue)
    {
        DynamicVars.Power<MiasmaPower>().BaseValue = baseValue;
    }
}