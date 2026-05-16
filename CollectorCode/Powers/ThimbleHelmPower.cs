using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Powers;

public class ThimbleHelmPower : CollectorPowerModel
{
    public ThimbleHelmPower()
    {
        WithTip(StaticHoverTip.Block);
    }

    public override decimal ModifyBlockAdditive(
        Creature target,
        decimal block,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (cardSource == null) return 0;
        var player = cardSource.Owner;
        if (player.Creature != Owner) return 0M;
        var creature = CollectorCmd.Torchhead(cardSource.Owner);
        if (creature is not { IsAlive: true }) return 0M;
        return !props.IsPoweredCardOrMonsterMoveBlock() ? 0M : Amount;
    }
}