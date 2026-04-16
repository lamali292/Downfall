using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Collector.Token;
using Downfall.Code.Commands;
using Downfall.Code.Events;
using Downfall.Code.Piles;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;

namespace Downfall.Code.Relics.Collector;

[Pool(typeof(CollectorRelicPool))]
public class KrampianCoal : CollectorRelicModel, IAfterCustomDraw
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<LuckyWick>()];

    public async Task AfterCustomDraw(Player player, PileType pile, CardPileAddResult result)
    {
        if (player != Owner || pile != CollectorPile.Collected || result.success) return;
        await DownfallCardCmd.GiveCard<LuckyWick>(player, PileType.Hand);
    }
}