using BaseLib.Utils;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Events;

namespace Snecko.SneckoCode.Relics;

[Pool(typeof(SneckoRelicPool))]
public class LoadedDie : SneckoRelicModel, IAfterCardMuddled
{
    public LoadedDie() : base(RelicRarity.Uncommon)
    {
        WithTip(SneckoKeywords.Muddle);
        WithTip(StaticHoverTip.Block);
    }


    public async Task AfterCardMuddled(PlayerChoiceContext ctx, CardModel card, AbstractModel? source)
    {
        if (card.Owner != Owner) return;
        var cost = card.EnergyCost.GetAmountToSpend();
        if (cost == 0) return;
        Flash();
        await CreatureCmd.GainBlock(Owner.Creature, cost, BlockProps.nonCardUnpowered, null);
    }
}