using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Relics;

[Pool(typeof(SneckoRelicPool))]
public class RareBoosterBox() : SneckoRelicModel(RelicRarity.Shop)
{
    public override bool HasUponPickupEffect => true;

    public override async Task AfterObtained()
    {
        var card = SneckoModel.CreateRewardSneckoCards(Owner, 1, c => c.Rarity == CardRarity.Rare).FirstOrDefault();
        if (card == null) return;
        var result = await CardPileCmd.Add(card, PileType.Deck);
        CardCmd.PreviewCardPileAdd(result, 0.1f);
    }
}