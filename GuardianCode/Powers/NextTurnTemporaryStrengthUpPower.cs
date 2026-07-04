using BaseLib.Abstracts;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Powers;

public class NextTurnTemporaryStrengthUpPower : GuardianPowerModel
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player.Creature != Owner) return;
        await PowerCmd.Apply<NextTurnTemporaryStrengthUpPowerPower>(ctx, Owner, Amount, Owner, null);
        await PowerCmd.Remove(this);
    }
}

public class NextTurnTemporaryStrengthUpPowerPower : CustomTemporaryPowerModelWrapper<NextTurnTemporaryStrengthUpPower, StrengthPower>;