using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class LuckyBreak : SneckoCardModel
{
    public LuckyBreak() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(8, 3);
        WithCards(1);
        WithEnergy(2);
        WithCalculatedVar("Draws", 0, 1, ExpensiveCardsInHand);
    }

    private static decimal ExpensiveCardsInHand(CardModel card, Creature? _)
    {
        return card.Owner.Hand.Count(e => e != card && !e.EnergyCost.CostsX && e.EnergyCost.GetAmountToSpend() >= card.DynamicVars.Energy.BaseValue);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var draws = (int)((CalculatedVar)DynamicVars["Draws"]).Calculate(null);
        await CardPileCmd.Draw(ctx, draws, Owner);
    }
}