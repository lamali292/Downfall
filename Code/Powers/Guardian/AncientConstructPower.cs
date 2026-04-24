using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Powers.Guardian;

public class AncientConstructPower : GuardianPowerModel
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player.Creature != Owner) return;
        if (Owner.GetPowerAmount<ArtifactPower>() == 0)
        {
            await PowerCmd.Apply<ArtifactPower>(ctx, Owner, Amount, Owner, null);
        }
    }
}