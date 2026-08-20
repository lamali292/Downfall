using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Powers;

public class VenomPower() : SneckoPowerModel(PowerType.Debuff)
{
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext ctx, PowerModel power, decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power == this || power.Owner != Owner || power.GetTypeForAmount(amount) != PowerType.Debuff) return;
        await CompatibilityCreatureCmd.Damage(ctx, Owner, Amount, DamageProps.nonCardHpLoss, applier, null,
            null);
    }
}