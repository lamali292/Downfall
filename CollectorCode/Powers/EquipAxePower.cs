using Collector.CollectorCode.Core;
using Collector.CollectorCode.Events;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Powers;

public class EquipAxePower() : CollectorPowerModel(PowerType.Buff, PowerStackType.Single), IShouldTorchheadTargetAll
{
    public bool ShouldTorchheadTargetAll(Player player) => player.Creature == Owner;

    public Task AfterShouldTorchheadTargetAll(PlayerChoiceContext ctx, Player player)
    {
        Flash();
        return Task.CompletedTask;
    }
}