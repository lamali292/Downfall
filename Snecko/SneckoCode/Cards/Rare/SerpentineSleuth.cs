using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class SerpentineSleuth : SneckoCardModel
{
    public SerpentineSleuth() : base(4, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithGift(new Gift
        {
            Rarity = CardRarity.Rare,
            Type = CardType.Power
        });
    }

    // TODO: Implement
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
    }
}