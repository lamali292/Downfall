using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class Glut : SneckoCardModel
{
    public Glut() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(4, 2);
        WithCalculatedVar("Repeat", 0, Calc);
    }

    private static decimal Calc(CardModel card, Creature? _)
    {
        return card.Owner.Hand.Count(e => e != card);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var hits = (int)((CalculatedVar)DynamicVars["Repeat"]).Calculate(cardPlay.Target);
        await CommonActions.CardAttack(this, cardPlay, hits).Execute(ctx);
    }
}