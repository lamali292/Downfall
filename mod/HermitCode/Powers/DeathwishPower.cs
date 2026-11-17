using Hermit.HermitCode.Core;
using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.Events;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Powers;

public class DeathwishPower : HermitPowerModel, IShouldTriggerDeadOn
{
    public DeathwishPower()
    {
        WithTip(HermitKeywords.DeadOn);
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer != Owner || cardSource is null || !props.IsPoweredAttack() ||
            !HermitCmd.IsAdjacentToCurse(cardSource))
            return 0;

        return Amount;
    }

    public bool ShouldTriggerDeadOn(CardModel card)
    {
        return card.Owner.Creature == Owner && HermitCmd.IsAdjacentToCurse(card);
    }
}