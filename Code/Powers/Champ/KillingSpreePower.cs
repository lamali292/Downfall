using Downfall.Code.Abstract;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Downfall.Code.Powers.Champ;

public class KillingSpreePower() : ChampPowerModel(PowerType.Buff, PowerStackType.Single), IIgnoreChampChargeCap
{
    public bool IgnoreChargeCap(Player player)
    {
        return player.Creature == Owner;
    }
}