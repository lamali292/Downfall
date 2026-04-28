using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Basic;

[Pool(typeof(SneckoCardPool))]
public class StrikeSnecko : SneckoCardModel
{
    public StrikeSnecko() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithTags(CardTag.Strike);
    }

    // TODO: Implement
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
    }


}