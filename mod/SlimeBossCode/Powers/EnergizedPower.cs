using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

public class EnergizedPower : SlimeBossPowerModel
{
    public override string CustomPackedIconPath => EnergyIconHelper.GetPath(this);

    public override string CustomBigIconPath => EnergyIconHelper.GetPath(this);

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return Owner == player.Creature ? amount + Amount : amount;
    }
}
