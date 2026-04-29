using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Powers;

public class InfernalFormPower : HexaghostPowerModel
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player.Creature != Owner) return;
        await PowerCmd.Apply<IntensityPower>(ctx, Owner, Amount, Owner, null);
    }
    
}