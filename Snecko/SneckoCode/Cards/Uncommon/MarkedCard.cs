using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class MarkedCard : SneckoCardModel
{
    public MarkedCard() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeywords(CardKeyword.Retain, CardKeyword.Exhaust);
        WithMuddle(1, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await SneckoCmd.MuddleHandCards(ctx, this, true);
    }
}