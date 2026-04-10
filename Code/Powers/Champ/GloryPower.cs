using Downfall.Code.Abstract;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Champ;

public class GloryPower : ChampPowerModel
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, CombatState combatState)
    {
        if (player.Creature != Owner) return;
        if (Amount < 10) return;
        await PowerCmd.Apply<UltimateStancePower>(Owner, 1, Owner, null);
        await PowerCmd.ModifyAmount(this, -10, Owner, null);
    }
}