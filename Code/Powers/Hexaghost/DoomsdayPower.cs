using Downfall.Code.Abstract;
using Downfall.Code.Core.Hexaghost;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Hexaghost;

public class DoomsdayPower : HexaghostPowerModel, IAfterGhostwheelAllIgnited
{
    public async Task AfterGhostwheelAllIgnited(PlayerChoiceContext ctx, Player player, GhostflameModel flame, int index)
    {
        if (player.Creature != Owner) return;
        await PowerCmd.Apply<DoomsArrivalPower>(ctx, Owner, Amount, Owner, null);
        await PowerCmd.Remove(this);
    }
}