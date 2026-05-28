using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Piles;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Events;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class KrampianCoal : CollectorRelicModel, IAfterCustomDraw
{
    public KrampianCoal() : base(RelicRarity.Shop)
    {
        WithTip<LuckyWick>();
    }

    public async Task AfterCustomDraw(Player player, PileType pile, CardPileAddResult result)
    {
        if (player != Owner || pile != CollectorPile.Collected || result.success) return;
        await DownfallCardCmd.GiveCard<LuckyWick>(player, PileType.Hand);
    }
}