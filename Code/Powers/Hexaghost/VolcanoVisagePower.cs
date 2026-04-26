using Downfall.Code.Abstract;
using Downfall.Code.Commands;
using Downfall.Code.Core.Hexaghost;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Hexaghost;

public class VolcanoVisagePower : HexaghostPowerModel, IAfterGhostwheelIgnited
{
    public VolcanoVisagePower()
    {
        WithTip(typeof(SoulBurnPower));
    }
    
    
    public async Task AfterGhostwheelIgnited(PlayerChoiceContext ctx, Player player, GhostflameModel flame, int index)
    {
        if (player.Creature != Owner) return;
        await PowerCmd.Apply<SoulBurnPower>(ctx, CombatState.HittableEnemies, Amount, Owner, null);
        Flash();
    }
}