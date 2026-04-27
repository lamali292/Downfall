using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Powers;

public class DoomsdayPower : HexaghostPowerModel, IAfterGhostwheelAllIgnited
{
    public async Task AfterGhostwheelAllIgnited(PlayerChoiceContext ctx, Player player, GhostflameModel flame, int index)
    {
        if (player.Creature != Owner) return;
        await PowerCmd.Apply<DoomsArrivalPower>(ctx, Owner, Amount, Owner, null);
        await PowerCmd.Remove(this);
    }
}