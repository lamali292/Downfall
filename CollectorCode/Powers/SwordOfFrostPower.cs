using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Collector.CollectorCode.Powers;

public class SwordOfFrostPower : CollectorPowerModel
{
    public SwordOfFrostPower()
    {
        WithTip<Burn>();
        WithTip<Ember>();
    }

    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (creator != null && creator.Creature == Owner)
        {
            if (card is Burn || card is Ember)
            {
                if (CombatManager.Instance.History.Entries.OfType<CardGeneratedEntry>().Count(
                        (Func<CardGeneratedEntry, bool>)(e =>
                            e.HappenedThisTurn(card.CombatState) && e.Actor == card.Owner.Creature &&
                            (e.Card is Burn || e.Card is Ember))) == 1)
                {
                    Flash();
                    await CardCmd.Exhaust(new ThrowingPlayerChoiceContext(), card);
                    await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), Amount, creator);
                }
            }
        }
    }
}