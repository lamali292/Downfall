using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class Shed : SneckoCardModel
{
    public Shed() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithBlock(5, 2);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var cards = Owner.GetHand();
        await SneckoCmd.Muddle(ctx, cards, this);
        var nowNull = cards.Count(e => e.EnergyCost.GetResolved() == 0);
        for (var i = 0; i < nowNull; i++)
        {
            await CommonActions.CardBlock(this, cardPlay);
        }
    }
}