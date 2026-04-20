using Downfall.Code.Abstract;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Guardian;

public class TimeSifterPower : GuardianPowerModel
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (player.Creature != Owner) return;
        await GuardianCmd.Accelerate(player, Amount);
    }
}