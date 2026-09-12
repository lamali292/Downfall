using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Downfall.DownfallCode.Powers;

public sealed class DrainedPower : DownfallPowerModel
{
    public DrainedPower() : base(PowerType.Debuff)
    {
        WithEnergyTip();
    }

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player) return;
        await PlayerCmd.LoseEnergy(Amount, player);
        await PowerCmd.Remove(this);
    }
}