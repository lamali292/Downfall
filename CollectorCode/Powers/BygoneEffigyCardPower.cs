using BaseLib.Extensions;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Extensions;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Powers;

public class BygoneEffigyCardPower : CollectorPowerModel
{

    public BygoneEffigyCardPower()
    {
        WithReserve(0);
        WithPower<StrengthPower>(0);
    }
    
    
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    // before plated armor
    public override async Task BeforeSideTurnEndEarly(
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
            await CollectorCmd.GetReserve(this);
            await MyCommonActions.ApplySelf<StrengthPower>(ctx, this);
            await PowerCmd.Remove(this);
        }
    }
    
    public void SetEffect(decimal baseValue)
    {
        DynamicVars.Reserve.BaseValue = baseValue;
        DynamicVars.Power<StrengthPower>().BaseValue = baseValue;
    }
}