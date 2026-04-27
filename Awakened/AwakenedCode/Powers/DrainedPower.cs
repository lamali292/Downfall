using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Powers;

public class DrainedPower : AwakenedPowerModel
{
    public DrainedPower() : base(PowerType.Debuff)
    {
        WithEnergyTip();
    }

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player || Owner.CombatState == null || LocalContext.NetId == null)
            return;
        await PlayerCmd.LoseEnergy(Amount, player);
        var ctx = new HookPlayerChoiceContext(
            player,
            LocalContext.NetId.Value,
            GameActionType.Combat);
        await AwakenedHook.OnDrained(Owner.CombatState, ctx, Owner.Player, Amount);
        await PowerCmd.Remove(this);
    }
}