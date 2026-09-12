using Collector.CollectorCode.Core;
using Collector.CollectorCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Powers;

public class EquipDaggerPower : CollectorPowerModel
{
    public EquipDaggerPower()
    {
        WithTip<PoisonPower>();
    }
    
    public override async Task AfterAttack(PlayerChoiceContext ctx, AttackCommand command)
    {
        if (Owner.Player == null || command.Attacker != Owner.Player?.Torchhead) return;
        foreach (var damageResult in command.Results.SelectMany(e => e))
        {
            Flash();
            await PowerCmd.Apply<PoisonPower>(ctx, damageResult.Receiver, Amount, Owner, null);
        }
    }
}