using Collector.CollectorCode.Core;
using Collector.CollectorCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Powers;

public class EquipShieldPower : CollectorPowerModel
{

    public EquipShieldPower()
    {
        WithTip(StaticHoverTip.Block);
    }

    public override async Task AfterAttack(PlayerChoiceContext ctx, AttackCommand command)
    {
        if (Owner.Player == null || command.Attacker != Owner.Player?.Torchhead) return;
        // command.Results is one entry per hit, each containing one DamageResult per target
        // that hit reached grant Block once per hit, not once per enemy hit.
        var hits = command.Results.Count();
        for (var i = 0; i < hits; i++)
        {
            Flash();
            await CreatureCmd.GainBlock(Owner, Amount, BlockProps.nonCardUnpowered, null);
        }
    }
}