using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Powers;

public class DoomedAttackPower : CollectorPowerModel
{
    public override async Task AfterAttack(PlayerChoiceContext ctx, AttackCommand command)
    {
        if (command.Attacker != Owner) return;
        foreach (var result in command.Results.SelectMany(r => r))
            await PowerCmd.Apply<CollectorDoomPower>(ctx, result.Receiver, Amount, Owner, null);

        await PowerCmd.Remove(this);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Side) return;
        await PowerCmd.Remove(this);
    }
}