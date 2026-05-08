using Downfall.DownfallCode.Powers;

using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Powers;

public class SharpHidePower : GuardianPowerModel
{
    protected override async Task AfterSideTurnStart(PlayerChoiceContext ctx, CombatSide side, ICombatState combatState)
    {
        if (side != Owner.Side) return;
        await PowerCmd.Apply<TemporaryThornsPower>(ctx, Owner, Amount, Owner, null, true);
    }
}