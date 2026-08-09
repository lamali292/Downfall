using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gremlins.GremlinsCode.Powers;

public class BubbleBarrierPower : GremlinsPowerModel
{
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player) return;
        await CreatureCmd.GainBlock(Owner, Amount, BlockProps.nonCardUnpowered, null);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Side) return;
        await PowerCmd.Remove(this);
    }
}