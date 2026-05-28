using Downfall.DownfallCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Powers;

public class RollThroughPower : SlimeBossPowerModel, IModifySelfDamage
{

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    => dealer == Owner && cardSource != null && cardSource.Tags.Contains(SlimeBossTag.Tackle) && props.HasFlag(ValueProp.Unpowered)? -amount : 0;

    public decimal ModifySelfDamage(decimal amount, AbstractModel model)
     =>  model is CardModel card && card.Tags.Contains(SlimeBossTag.Tackle) && card.Owner.Creature == Owner ? -
         0 : amount;

    public Task AfterModifyingSelfDamage(AbstractModel model)
        => PowerCmd.Decrement(this);
}