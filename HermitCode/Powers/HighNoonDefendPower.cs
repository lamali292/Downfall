using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Powers;

public class HighNoonDefendPower : HermitPowerModel
{
    private bool IsMyBasicDefend(CardModel card) => card.Owner.Creature == Owner &&
                                                    card.Tags.Contains(CardTag.Defend) &&
                                                    card.Rarity == CardRarity.Basic;
    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (!IsMyBasicDefend(card))
            return Task.CompletedTask;
        card.BaseReplayCount += Amount;
        return Task.CompletedTask;
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power != this) return Task.CompletedTask;
        var cards = Owner.Player?.PlayerCombatState?.AllCards.Where(IsMyBasicDefend);
        if (cards == null) return Task.CompletedTask;
        foreach (var card in cards)
            card.BaseReplayCount += (int) amount;
        return Task.CompletedTask;
    }
}