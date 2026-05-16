using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Awakened.AwakenedCode.Powers;

public class ManaburnPower() : AwakenedPowerModel(PowerType.Debuff), IOnDrained
{
    public async Task OnDrained(PlayerChoiceContext ctx, Player player, int amount)
    {
        if (Applier != player.Creature || LocalContext.NetId == null) return;
        await CreatureCmd.Damage(ctx,
            Owner, Amount * amount,
            ValueProp.Move | ValueProp.Unblockable | ValueProp.Unpowered, player.Creature);
    }
}