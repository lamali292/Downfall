using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Events;

public interface IShouldTorchheadTargetAll
{
    bool ShouldTorchheadTargetAll(Player player);
    Task AfterShouldTorchheadTargetAll(PlayerChoiceContext ctx, Player player);
}

