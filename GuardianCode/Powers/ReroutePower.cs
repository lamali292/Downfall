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
    private bool _hasBeenApplied;

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        _cardSource = cardSource;
        _hasBeenApplied = false;
        return Task.CompletedTask;
    }


    public override (PileType, CardPilePosition) ModifyCardPlayResultPileTypeAndPosition(
        CardModel card, bool isAutoPlay,
        ResourceInfo resources, PileType pileType, CardPilePosition position)
    {
        if (_cardSource == card || card.Keywords.Contains(CardKeyword.Exhaust) || card is not { Type: CardType.Attack or CardType.Skill }) return (pileType, position);

        _hasBeenApplied = true;
        var player = card.Owner;
        var stasisPile = GuardianCombatModel.GetOrInitStasis(player);
        if (stasisPile.Cards.Count >= GuardianCmd.GetMaxStasisSlots(player)) return (pileType, position);
        GuardianCmd.SetStasisCounter(card);
        card.EnergyCost.AfterCardPlayedCleanup();
        return (stasisPile.Type, position);
    }

    public override async Task AfterModifyingCardPlayResultPileOrPosition(CardModel card, PileType pileType,
        CardPilePosition position)
    {
        if (!_hasBeenApplied) return;
        _hasBeenApplied = false;
        await PowerCmd.Decrement(this);
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        return PowerCmd.Remove(this);
    }
}