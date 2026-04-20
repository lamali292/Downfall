using Downfall.Code.Abstract;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Powers.Guardian;

public class ReroutePower : GuardianPowerModel
{
    
    private CardModel? _cardSource;
    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        _cardSource = cardSource;
        return Task.CompletedTask;
    }
    
    
    public override (PileType, CardPilePosition) ModifyCardPlayResultPileTypeAndPosition(
        CardModel card, bool isAutoPlay,
        ResourceInfo resources, PileType pileType, CardPilePosition position)
    {
        if (_cardSource == card) return (pileType, position);
        var player = card.Owner;
        var pile = GuardianCmd.GetStasisPile(player);
        if (pile == null || !GuardianCmd.CanPutIntoStasis(player))
        {
            return (pileType, position);
        }
        GuardianCmd.SetStasisCounter(card);
        return (pile.Type, position);
    }
    
    public override async Task AfterModifyingCardPlayResultPileOrPosition(
        CardModel card, PileType pileType, CardPilePosition position)
    {
        await PowerCmd.Decrement(this);
    }

    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        return PowerCmd.Remove(this);
    }
}