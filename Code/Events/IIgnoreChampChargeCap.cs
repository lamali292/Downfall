using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.Code.Events;

public interface IIgnoreChampChargeCap
{
    bool IgnoreChargeCap(Player player);
}