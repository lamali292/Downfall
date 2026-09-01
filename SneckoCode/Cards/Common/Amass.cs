using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class Amass : SneckoCardModel
{
    public Amass() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithCalculatedBlock(9, Calc, DamageProps.card, 4);
    }

    private static decimal Calc(CardModel card, Creature? creature)
    {
        return card.Owner.Hand.Sum(e => e.EnergyCost.GetResolved());
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }
}