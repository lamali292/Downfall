using Downfall.Code.Abstract;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Awakened;

public class SpellbinderPower : AwakenedPowerModel
{
    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        CombatState combatState)
    {
        if (player != Owner.Player) return;
        for (var i = 0; i < Amount; i++) await AwakenedCmd.Conjure(player, combatState);
    }
}