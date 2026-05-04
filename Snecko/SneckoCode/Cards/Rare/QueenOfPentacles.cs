using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class QueenOfPentacles : SneckoCardModel
{
    public QueenOfPentacles() : base(3, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithGift(new Gift
        {
            IsDebuff = true
        });
    }

    // TODO: Implement
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
    }
}