using BaseLib.Abstracts;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Powers;

public class SharpHidePower : GuardianPowerModel
{
    protected override async Task AfterSideTurnStart(PlayerChoiceContext ctx, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side) return;
        await PowerCmd.Apply<SharpHideThornsPower>(ctx, Owner, Amount, Owner, null, true);
    }
}

public class SharpHideThornsPower : CustomTemporaryPowerModelWrapper<SharpHidePower, ThornsPower>
{
    protected override bool UntilEndOfOtherSideTurn => true;
}