using Champ.ChampCode.Core;
using Champ.ChampCode.Events;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Powers;

public class KillingSpreePower() : ChampPowerModel(PowerType.Buff, PowerStackType.Single), IIgnoreChampChargeCap
{
    public bool IgnoreChargeCap(Player player)
    {
        return player.Creature == Owner;
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (applier?.Player == null || applier != Owner) return Task.CompletedTask;
        ChampModel.GetStanceModel(applier.Player).ResetCharges();
        ChampModel.RefreshDisplay(applier.Player);

        return Task.CompletedTask;
    }
}