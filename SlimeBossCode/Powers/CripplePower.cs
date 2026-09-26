using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

// "Whenever ANY player deals attack damage to this enemy, if the enemy is Weak, that player gains Block."
public class CripplePower : SlimeBossPowerModel
{
    public override async Task AfterAttack(PlayerChoiceContext ctx, AttackCommand command)
    {
        var attacker = command.Attacker;
        if (attacker?.Player == null) return;
        if (command.Results.SelectMany(r => r).All(e => e.Receiver != Owner)) return;
        if (!Owner.GetPowerInstances<WeakPower>().Any(w => w.Amount > 0)) return;
        await CreatureCmd.GainBlock(attacker, Amount, BlockProps.nonCardUnpowered, null);
    }
}
