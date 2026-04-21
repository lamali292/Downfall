using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;

namespace Downfall.Code.Powers.Collector;

public class CantTouchThisPower : CollectorPowerModel
{
    public override async Task AfterAttack(AttackCommand command)
    {
        if (command.Attacker == null) return;
        /*
        foreach (var commandResult in command.Results)
        {
            if (commandResult.Receiver != Owner) continue;
            if (commandResult.WasFullyBlocked)
            {
                await PowerCmd.Apply<CollectorDoomPower>(command.Attacker, Amount, Owner, null);
            }
        }
        */
        var list = command.Results.Where(r => r.Receiver == Owner).ToList();
        if (list.Count != 0 && list.All(r => r.WasFullyBlocked))
            await PowerCmd.Apply<CollectorDoomPower>(command.Attacker, Amount, Owner, null);
    }
}