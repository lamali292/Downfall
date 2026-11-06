using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

public class OverexertPower : SlimeBossPowerModel
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player.Creature != Owner) return;
        await PowerCmd.Decrement(this);
        if (Amount > 0) return;
        await SlimeBossCmd.AbsorbAll(ctx, player);
    }
}