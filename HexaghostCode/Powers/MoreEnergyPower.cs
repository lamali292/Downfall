using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Hexaghost.HexaghostCode.Powers;

public class MoreEnergyPower : HexaghostPowerModel
{
    public override async Task AfterEnergyReset(Player player)
    {
        if (player.Creature != Owner) return;
        Flash();
        await PlayerCmd.GainEnergy(Amount, player);
    }
}