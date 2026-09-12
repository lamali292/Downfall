using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Powers;

public class BookOfStabbingPower : CollectorPowerModel
{
    public BookOfStabbingPower()
    {
        WithTip<MiasmaPower>();
    }
    
    public override async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
     
        if (dealer == null || dealer != Owner && dealer.PetOwner?.Creature != Owner || !props.IsPoweredAttack() || result.UnblockedDamage <= 0)
            return;
        await PowerCmd.Apply<MiasmaPower>(choiceContext, target, Amount, Owner, null);
    }
}