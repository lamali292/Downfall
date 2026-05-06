using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Events;

namespace Snecko.SneckoCode.Powers;

public class MudShieldPower : SneckoPowerModel, IAfterCardMuddled
{
    public MudShieldPower()
    {
        WithTip(StaticHoverTip.Block);
        WithTip(SneckoKeywords.Muddle);
    }


    public async Task AfterCardMuddled(PlayerChoiceContext ctx, CardModel card, AbstractModel? source)
    {
        if (card.Owner.Creature != Owner) return;
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
    }
}