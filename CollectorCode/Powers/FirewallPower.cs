using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Powers;

public class FirewallPower : CollectorPowerModel
{
    public FirewallPower()
    {
        WithTip(StaticHoverTip.Block);
    }

    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        if (card.Owner.Creature != Owner || amount <= 0) return;
        Flash();
        await CreatureCmd.GainBlock(Owner, Amount * amount, BlockProps.nonCardUnpowered, null);
    }
}
