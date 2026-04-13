using Downfall.Code.Abstract;
using Downfall.Code.Cards.Collector.Token;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Powers.Collector;

public class DoubleTroublePower : CollectorPowerModel
{
    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        return card.Owner.Creature != Owner || card is not ICollectible ? playCount : playCount + 1;
    }

    public override async Task AfterModifyingCardPlayCount(CardModel card)
    {
        await PowerCmd.Decrement(this);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side)
            return;
        await PowerCmd.Remove(this);
    }
}