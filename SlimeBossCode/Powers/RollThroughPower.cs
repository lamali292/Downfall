using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Powers;

public class RollThroughPower : SlimeBossPowerModel, IModifySelfDamage
{

    public decimal ModifySelfDamage(decimal amount, AbstractModel model)
    {
        return model is CardModel card && card.Tags.Contains(SlimeBossTag.Tackle) && card.Owner.Creature == Owner
            ? 0
            : amount;
    }

    public Task AfterModifyingSelfDamage(AbstractModel model)
    {
        return PowerCmd.Decrement(this);
    }
}