using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Piles;
using Automaton.AutomatonCode.Vfx;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Powers;

public class BronzeOrbPower : AutomatonPowerModel, IModifyCardPlayResultLocation
{
    //// todo make fail if stash is full and still tick down, make tick down even if the card has encode etc
    public CardLocationCompatiblity ModifyCardPlayResultLocationCompability(CardModel card, bool isAutoPlay,
        ResourceInfo resources, CardLocationCompatiblity cardLocation)
    {
        if (card.Owner.Creature != Owner || card.Keywords.Contains(CardKeyword.Exhaust) ||
            AutomatonCmd.IsEncodable(card) || card.Type is CardType.Power) return cardLocation;
        NStashDisplay.EnsureFor(card.Owner);
        return new CardLocationCompatiblity(card.Owner, StashPile.Stash, CardPilePosition.Top);
    }

    public Task AfterModifyingCardPlayResultLocationCompability(CardModel card, CardLocationCompatiblity cardLocation)
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