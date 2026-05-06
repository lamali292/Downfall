using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class DiscountSale : SneckoCardModel
{
    public DiscountSale() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithCards(2);
        WithMuddle(1, 1);
        WithKeyword(CardKeyword.Exhaust);

    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Draw(this, ctx);
        await SneckoCmd.MuddleHandCards(ctx, this);
    }
}