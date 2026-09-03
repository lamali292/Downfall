using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class ThimbleHelm : CollectorRelicModel
{
    public ThimbleHelm() : base(RelicRarity.Rare)
    {
        WithBlock(1);
    }

    public override decimal ModifyBlockAdditive(
        Creature target,
        decimal block,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (cardSource?.Owner != Owner) return 0;
        if (Owner.Torchhead is not { IsAlive: true }) return 0;
        return props.IsPoweredCardOrMonsterMoveBlock() ? DynamicVars.Block.IntValue : 0;
    }

    public override Task AfterModifyingBlockAmount(decimal modifiedAmount, CardModel? cardSource, CardPlay? cardPlay)
    {
        Flash();
        return Task.CompletedTask;
    }
}