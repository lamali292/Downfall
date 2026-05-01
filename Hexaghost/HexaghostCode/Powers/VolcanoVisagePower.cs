using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Powers;

public class VolcanoVisagePower : HexaghostPowerModel, IAfterGhostflameIgnited
{
    public VolcanoVisagePower()
    {
        WithTip(typeof(SoulBurnPower));
    }


    public async Task AfterGhostflameIgnited(PlayerChoiceContext ctx, Player player, GhostflameModel flame, int index)
    {
        if (player.Creature != Owner) return;
        await PowerCmd.Apply<SoulBurnPower>(ctx, CombatState.HittableEnemies, Amount, Owner, null);
        Flash();
    }
}