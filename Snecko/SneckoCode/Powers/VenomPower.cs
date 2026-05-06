using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Powers;

public class VenomPower : SneckoPowerModel
{
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext ctx, PowerModel power, decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power == this || power.Owner != Owner || power.Type != PowerType.Debuff || amount <= 0) return;
        await CreatureCmd.Damage(ctx, Owner, Amount, ValueProp.Unblockable | ValueProp.Unpowered, applier, null);
    }
}