using Champ.ChampCode.Core;
using Champ.ChampCode.Events;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Champ.ChampCode.Powers;

public class KillingSpreePower() : ChampPowerModel(PowerType.Buff, PowerStackType.Single), IIgnoreChampChargeCap
{
    public bool IgnoreChargeCap(Player player)
    {
        return player.Creature == Owner;
    }
}