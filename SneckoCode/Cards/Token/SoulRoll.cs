using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Token;

[Pool(typeof(SneckoCardPool))]
public class SoulRoll : SneckoCardModel
{
    // Todo: fix that it should be not offclass card but in no rewards and no basic
    public SoulRoll() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithKeywords(CardKeyword.Retain, CardKeyword.Exhaust);
        WithBlock(3, 3);
        WithMuddle(1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await SneckoCmd.MuddleHandCards(ctx, this);
    }
}