using Awakened.AwakenedCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Powers;

public class SchemePower : AwakenedPowerModel
{
    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (card.Owner.Creature == Owner && card.EnergyCost.GetAmountToSpend() <= 1) return playCount + 1;
        return playCount;
    }

    public override async Task AfterModifyingCardPlayCount(CardModel card)
    {
        await PowerCmd.Decrement(this);
    }
    
    public override async Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner)) return;
        await PowerCmd.Remove(this);
    }
}