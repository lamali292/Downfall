using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

public class RallyTheTroopsPower : SlimeBossPowerModel
{
    // The card whose play applied this power; its own play must not trigger the power.
    private CardModel? _applyingCard;

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount,
        Creature? applier, CardModel? cardSource)
    {
        if (power == this && _applyingCard == null) _applyingCard = cardSource;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner) return;
        if (cardPlay.Card == _applyingCard)
        {
            _applyingCard = null;
            return;
        }
        await SlimeBossCmd.Command(ctx, cardPlay.Card.Owner, 1, false);
        Flash();
        await PowerCmd.Decrement(this);
    }


    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        return participants.Contains(Owner) ? PowerCmd.Remove(this) : Task.CompletedTask;
    }
}