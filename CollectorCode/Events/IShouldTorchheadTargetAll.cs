using MegaCrit.Sts2.Core.Entities.Players;

namespace Collector.CollectorCode.Events;

public interface IShouldTorchheadTargetAll
{
    bool ShouldTorchheadTargetAll(Player player);
}

