using Downfall.Code.Abstract;
using Downfall.Code.Core.Collector;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Collector;

public class PyromancyPower : CollectorPowerModel
{
    public override Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player.Creature != Owner) return Task.CompletedTask;
        CollectorEnergy.Gain(player, Amount);
        return Task.CompletedTask;
    }
}