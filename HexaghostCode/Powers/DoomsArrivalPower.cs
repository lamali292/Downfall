using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Hexaghost.HexaghostCode.Powers;

public class DoomsArrivalPower : HexaghostPowerModel
{
    public override bool ShouldTakeExtraTurn(Player player)
    {
        return player.Creature == Owner;
    }

    public override async Task AfterTakingExtraTurn(Player player)
    {
        Flash();
        await PowerCmd.TickDownDuration(this);
    }
}