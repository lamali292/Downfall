using MegaCrit.Sts2.Core.Entities.Players;

namespace Collector.CollectorCode.Events;

public interface IPreventCollectedDraw
{
    bool PreventCollectedDraw(Player player);
}