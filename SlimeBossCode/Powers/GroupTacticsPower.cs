using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

// "EVERYONE draws 1 additional card at the start of their turn."
public class GroupTacticsPower : SlimeBossPowerModel
{
    public override Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        return CardPileCmd.Draw(ctx, Amount, player);
    }
}
