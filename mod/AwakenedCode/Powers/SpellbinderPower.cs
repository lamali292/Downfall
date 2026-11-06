using Awakened.AwakenedCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Powers;

public class SpellbinderPower : AwakenedPowerModel
{
    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player != Owner.Player) return;
        for (var i = 0; i < Amount; i++) await AwakenedCmd.Conjure(player);
    }
}