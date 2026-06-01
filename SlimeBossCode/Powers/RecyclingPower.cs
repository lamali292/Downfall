using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

public class RecyclingPower : SlimeBossPowerModel
{
    public override Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        return player.Creature == Owner ? SlimeBossCmd.Slurp(player, Amount) : Task.CompletedTask;
    }
}