using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Powers;

public class CantTouchThisPower : CollectorPowerModel
{
    public override async Task AfterAttack(PlayerChoiceContext ctx, AttackCommand command)
    {
        if (command.Attacker == null) return;
        /*
        foreach (var commandResult in command.Results)
        {
            if (commandResult.Receiver != Owner) continue;
            if (commandResult.WasFullyBlocked)
            {
                await PowerCmd.Apply<CollectorDoomPower>(ctx,command.Attacker, Amount, Owner, null);
            }
        }
        */
        var list = command.Results.SelectMany(r => r).Where(r => r.Receiver == Owner).ToList();
        if (list.Count != 0 && list.All(r => r.WasFullyBlocked))
            await PowerCmd.Apply<CollectorDoomPower>(ctx, command.Attacker, Amount, Owner, null);
    }
}