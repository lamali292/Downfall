using MegaCrit.Sts2.Core.Entities.Players;

namespace Collector.CollectorCode.Events;

/// <summary>Changes the HP Torchhead is rekindled with when a card commands it to attack while it is down.</summary>
public interface IModifyTorchheadRekindle
{
    int ModifyTorchheadRekindle(Player player, int current);
}
