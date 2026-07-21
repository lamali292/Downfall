using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

public class OozeBathPower : SlimeBossPowerModel
{
    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    protected override async Task AfterSideTurnStart(PlayerChoiceContext ctx, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side) return;
        await PowerCmd.Apply<GoopPower>(ctx, Owner, Amount, Applier, null);
    }
}