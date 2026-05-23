using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.DownfallCode.Powers;

public class StunnedPower() : DownfallPowerModel(PowerType.Debuff)
{
    public override Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player.Creature == Owner)
        {
            Flash();
            PlayerCmd.EndTurn(player, false);
            SetAmount(Amount - 1);
        }

        return Task.CompletedTask;
    }
}