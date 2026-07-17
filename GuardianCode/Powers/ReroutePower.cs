using Downfall.DownfallCode.Compatibility;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Powers;

public class ReroutePower : GuardianPowerModel, IModifyCardPlayResultLocation
{
    private CardModel? _cardSource;

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        _cardSource = cardSource;
        return Task.CompletedTask;
    }
    

    public CardLocationCompatiblity ModifyCardPlayResultLocationCompability(CardModel card, bool isAutoPlay,
        ResourceInfo resources, CardLocationCompatiblity cardLocation)
    {
        var player = card.Owner;
        if (_cardSource == card || card.Keywords.Contains(CardKeyword.Exhaust) || card is not { Type: CardType.Attack or CardType.Skill } || player.Creature != Owner)
            return cardLocation;

        var stasisPile = GuardianCombatModel.GetOrInitStasis(player);
        return stasisPile.Cards.Count >= GuardianCmd.GetMaxStasisSlots(player) ? cardLocation : new CardLocationCompatiblity(card.Owner, stasisPile.Type, CardPilePosition.Bottom);
    }

    public async Task AfterModifyingCardPlayResultLocationCompability(CardModel card, CardLocationCompatiblity cardLocation)
    {
        GuardianCmd.SetStasisCounter(card);
        card.EnergyCost.AfterCardPlayedCleanup();
        await PowerCmd.Decrement(this);
    }
    

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        return PowerCmd.Remove(this);
    }
}