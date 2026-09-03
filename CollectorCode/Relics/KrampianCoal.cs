using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Events;
using Downfall.DownfallCode.Abstract;
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
        WithTip(CollectorKeyword.Pyre);
        WithTip<LuckyWick>();
        WithUpgradedCardTip<LuckyWick>();

    }
    
    public async Task OnPyre(PlayerChoiceContext ctx, CardModel card, CardModel pyred)
    {
        if (pyred.Type is CardType.Curse or CardType.Status)
        {
            var willUpgrade = pyred.Type == CardType.Curse;
            await DownfallCardCmd.GiveCard<LuckyWick>(Owner, PileType.Hand, upgraded: willUpgrade);
            Flash();
        }
    }
}