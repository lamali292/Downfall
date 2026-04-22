using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Powers.Guardian;

public class AncientConstructPower : GuardianPowerModel
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (player.Creature != Owner) return;
        if (Owner.GetPowerAmount<ArtifactPower>() == 0)
        {
            await PowerCmd.Apply<ArtifactPower>(Owner, Amount, Owner, null);
        }
    }
}