using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Cards.Rare;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

// "Whenever you play a card this turn, gain 1 Potency this turn." Self-removes at end of the owner's turn.
public class RallyTheTroopsPower : SlimeBossPowerModel
{
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner) return;
        await PowerCmd.Apply<RallyTheTroopsPotencyPower>(ctx, Owner, 1, Owner, null);
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        return participants.Contains(Owner) ? PowerCmd.Remove(this) : Task.CompletedTask;
    }
}

public class RallyTheTroopsPotencyPower : CustomTemporaryPowerModelWrapper<RallyTheTroops, PotencyPower>;
