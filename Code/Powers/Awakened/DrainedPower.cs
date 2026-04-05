using Downfall.Code.Abstract;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Downfall.Code.Powers.Awakened;

public class DrainedPower : AwakenedPowerModel
{
    public DrainedPower() : base(PowerType.Debuff)
    {
        WithEnergyTip();
    }

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player)
            return;
        await PlayerCmd.LoseEnergy(Amount, player);
        await DownfallHook.OnDrained(Owner.Player, Amount);
        await PowerCmd.Remove(this);
    }
}