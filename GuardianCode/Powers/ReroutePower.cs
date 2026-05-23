using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Powers;

public class ReroutePower : GuardianPowerModel
{
    private CardModel? _cardSource;

    protected override Task AfterApplied(PlayerChoiceContext ctx, Creature? applier, CardModel? cardSource)
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
        if (pile == null || !GuardianCmd.CanPutIntoStasis(player)) return (pileType, position);
        card.EnergyCost.AfterCardPlayedCleanup();
        GuardianCmd.SetStasisCounter(card);
        return (pile.Type, position);
    }

    protected override async Task AfterModifyingCardPlayResultPileOrPosition(PlayerChoiceContext ctx, 
        CardModel card, PileType pileType, CardPilePosition position)
    {
        await PowerCmd.Decrement(this);
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        return PowerCmd.Remove(this);
    }
}