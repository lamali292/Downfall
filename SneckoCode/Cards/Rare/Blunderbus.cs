using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class Blunderbus : SneckoCardModel
{
    public Blunderbus() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithDamage(8, 3);
        WithEnergy(3);
        WithCalculatedVar("Hits", 1, ExpensiveCardsInHand);
    }

    private static decimal ExpensiveCardsInHand(CardModel card, Creature? _)
    {
        return card.Owner.Hand.Count(e => e != card && e.EnergyCost.GetResolved() >= card.DynamicVars.Energy.BaseValue);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var hits = (int)((CalculatedVar)DynamicVars["Hits"]).Calculate(cardPlay.Target);
        await CommonActions.CardAttack(this, cardPlay, hits).Execute(ctx);
    }
}