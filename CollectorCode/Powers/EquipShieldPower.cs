using Collector.CollectorCode.Core;
using Collector.CollectorCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
    
    public override async Task BeforeSideTurnEndEarly(
        PlayerChoiceContext ctx,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || Owner.Player?.Torchhead?.IsAlive is null or false)
            return;
        Flash();
        await CreatureCmd.GainBlock(Owner, Amount, BlockProps.nonCardUnpowered, null);
    }
}