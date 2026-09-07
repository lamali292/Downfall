using BaseLib.Extensions;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Powers;

public class LagavulinMatriarchCardPower : CollectorPowerModel
{

    public LagavulinMatriarchCardPower()
    {
        WithPower<PlatedArmorPower>(0);
    }
    
    public void SetSecondAmount(decimal baseValue)
    {
        DynamicVars.Power<PlatedArmorPower>().BaseValue = baseValue;
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext ctx, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner)) return;
        Flash();
        await MyCommonActions.ApplySelf<PlatedArmorPower>(ctx, this);
        await PowerCmd.Decrement(this);
    }
}