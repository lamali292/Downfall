using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.Code.Events;

public interface IPreventCollectedDraw
{
    bool PreventCollectedDraw(Player player);
}