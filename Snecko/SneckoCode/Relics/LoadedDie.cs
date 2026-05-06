using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Events;

namespace Snecko.SneckoCode.Relics;

[Pool(typeof(SneckoRelicPool))]
public class LoadedDie : SneckoRelicModel, IAfterCardMuddled
{
    public LoadedDie() : base(RelicRarity.Common)
    {
        WithTip(SneckoKeywords.Muddle);
        WithBlock(1);
    }


    public async Task AfterCardMuddled(PlayerChoiceContext ctx, CardModel card, AbstractModel? source)
    {
        if (card.Owner != Owner) return;
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
    }
}