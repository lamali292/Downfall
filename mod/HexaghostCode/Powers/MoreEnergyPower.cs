using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Hexaghost.HexaghostCode.Powers;

public class MoreEnergyPower : HexaghostPowerModel
{
    public MoreEnergyPower()
    {
        WithEnergyTip();
    }

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return player != Owner.Player ? amount : amount + Amount;
    }
}