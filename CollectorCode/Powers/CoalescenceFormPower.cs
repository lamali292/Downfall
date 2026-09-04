using Collector.CollectorCode.Core;
using Collector.CollectorCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Collector.CollectorCode.Powers;

public class CoalescenceFormPower : CollectorPowerModel
{
    public CoalescenceFormPower()
    {
        WithReserve(1);
    }

    public override int DisplayAmount => DynamicVars.Reserve.IntValue;


    public override async Task AfterEnergyReset(Player player)
    {
        if (player.Creature != Owner)
            return;
        
        await CollectorCmd.GetReserve(player, DynamicVars.Reserve.IntValue);
        DynamicVars.Reserve.BaseValue += Amount;
        InvokeDisplayAmountChanged();
    }
}