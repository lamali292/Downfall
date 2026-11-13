using MegaCrit.Sts2.Core.Entities.Players;

namespace Champ.ChampCode.Events;

public interface IIgnoreChampChargeCap
{
    bool IgnoreChargeCap(Player player);
}