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
    public Amass() : base(3, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithCalculatedBlock(12, Calc, ValueProp.Move, 4);
    }

    private static decimal Calc(CardModel card, Creature? creature)
    {
        return card.Owner.GetHand().Sum(e => e.EnergyCost.GetResolved());
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }
}