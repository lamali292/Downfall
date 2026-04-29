using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Powers;

public class MoreEnergyPower : HexaghostPowerModel
{
    protected override async Task AfterEnergyReset(PlayerChoiceContext ctx, Player player)
    {
        if (player.Creature != Owner) return;
        Flash();
        await PlayerCmd.GainEnergy(Amount, player);
    }
}