using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Downfall;

public class StunnedPower() : DownfallPowerModel(PowerType.Debuff, PowerStackType.Single)
{
 
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        /*
        if (player.Creature != Owner) return;
        //await CreatureCmd.Stun(player.Creature);
        await PowerCmd.Remove(this);
        PlayerCmd.EndTurn(player, false);
        */
    }

    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return;
        //await CreatureCmd.Stun(player.Creature);
        await PowerCmd.Remove(this);
        PlayerCmd.EndTurn(player, false);
    }
    
}