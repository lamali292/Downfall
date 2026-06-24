using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Powers;

public class MagazinePower : HermitPowerModel
{
    public MagazinePower()
    {
        WithTip(CardKeyword.Retain);
    }
    
    
    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (!card.IsBasicStrikeOrDefend || card.Owner != Owner.Player)
            return Task.CompletedTask;
        CardCmd.ApplyKeyword(card, CardKeyword.Retain);
        return Task.CompletedTask;
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var cards = Owner.Player?.PlayerCombatState?.AllCards.Where(c => c.IsBasicStrikeOrDefend);
        if (cards == null) return Task.CompletedTask;
        foreach (var card in cards)
            CardCmd.ApplyKeyword(card, CardKeyword.Retain);
        return Task.CompletedTask;
    }

    private static bool IsBasicStrike(CardModel card) =>  card.Rarity == CardRarity.Basic && card.Tags.Contains(CardTag.Strike);

    
    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        return !props.IsPoweredAttack() || cardSource == null || !IsBasicStrike(cardSource) || dealer != Owner ||
               CombatManager.Instance.History.CardPlaysFinished
                   .Any(e => e.HappenedThisTurn(CombatState) && IsBasicStrike(e.CardPlay.Card) && e.CardPlay.Card.Owner.Creature == Owner) ? 0M : Amount;
    }
}