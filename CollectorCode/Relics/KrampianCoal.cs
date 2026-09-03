using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Events;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class KrampianCoal : CollectorRelicModel, IOnPyre
{
    public KrampianCoal() : base(RelicRarity.Rare)
    {
        WithTip<LuckyWick>();
        //WithTip<LuckyWick>(); Todo: Figure out how to show upgraded lucky wick.
    }

    /*
    public async Task AfterCustomDraw(Player player, PileType pile, CardPileAddResult result)
    {
        if (player != Owner || pile != CollectorPile.Collected || result.success) return;
        await DownfallCardCmd.GiveCard<LuckyWick>(player, PileType.Hand);
    }
    */
    
    public async Task OnPyre(PlayerChoiceContext ctx, CardModel card, CardModel pyred)
    {
        if (pyred.Type is CardType.Curse or CardType.Status)
        {
            var willUpgrade = (pyred.Type == CardType.Curse);
            await DownfallCardCmd.GiveCard<LuckyWick>(Owner, PileType.Hand, CardPilePosition.Bottom, willUpgrade);
            Flash();
        }
    }
}