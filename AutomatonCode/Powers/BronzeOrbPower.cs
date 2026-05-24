using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Piles;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Powers;

public class BronzeOrbPower : AutomatonPowerModel
{
    public override (PileType, CardPilePosition) ModifyCardPlayResultPileTypeAndPosition(CardModel card,
        bool isAutoPlay,
        ResourceInfo resources, PileType pileType, CardPilePosition position)
    {
        return card.Owner.Creature != Owner ? (pileType, position) : (StashPile.Stash, CardPilePosition.Top);
    }

    protected override Task AfterModifyingCardPlayResultPileOrPosition(PlayerChoiceContext ctx, CardModel card,
        PileType pileType,
        CardPilePosition position)
    {
        return PowerCmd.Decrement(this);
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Side) return Task.CompletedTask;
        PowerCmd.Remove(this);
        return Task.CompletedTask;
    }
}