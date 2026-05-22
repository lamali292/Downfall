using Downfall.DownfallCode.Abstract;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.DownfallCode.Powers;

public class StunnedPower() : DownfallPowerModel(PowerType.Debuff, PowerStackType.Counter)
{
    public override Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player.Creature == Owner) {
            Flash();
            PlayerCmd.EndTurn(player, canBackOut: false);
            SetAmount(Amount - 1);
        }
        return Task.CompletedTask;
    }
}