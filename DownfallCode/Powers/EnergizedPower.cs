using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;

namespace Downfall.DownfallCode.Powers;

public class EnergizedPower : DownfallPowerModel
{
    public override string CustomPackedIconPath => EnergyIconHelper.GetPath(this);

    public override string CustomBigIconPath => EnergyIconHelper.GetPath(this);

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return Owner == player.Creature ? amount + Amount : amount;
    }
}