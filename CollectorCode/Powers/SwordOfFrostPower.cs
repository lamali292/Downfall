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
        if (creator == null || creator.Creature != Owner) return;

        if (card is not (Burn or Ember)) return;
        if (CombatManager.Instance.History.Entries.OfType<CardGeneratedEntry>().Count(
                e =>
                    e.HappenedThisTurn(CombatState) && e.Actor == Owner &&
                    e.Card is Burn or Ember) != 1) return;

        Flash();
        await CardCmd.Exhaust(new BlockingPlayerChoiceContext(), card);
        await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), Amount, creator);
    }
}