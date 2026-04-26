using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.Code.Powers.Hexaghost;

public class DoomsArrivalPower : HexaghostPowerModel
{
    public override bool ShouldTakeExtraTurn(Player player) => player.Creature == Owner;
    public override async Task AfterTakingExtraTurn(Player player)
    {
        await PowerCmd.TickDownDuration(this);
    }
}