using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class SerpentsNest : SneckoCardModel
{
    public SerpentsNest() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithGift(new Gift
        {
            Rarity = CardRarity.Uncommon,
            Type = CardType.Power,
        });
    }

    // TODO: Implement
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
    }
}