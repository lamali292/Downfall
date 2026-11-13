using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Events;

namespace SlimeBoss.SlimeBossCode.Powers;

public class MinionMasterPower : SlimeBossPowerModel, IModifyConsumeCount
{
    public int ModifyConsumeCount(Player player, int amount, CardModel? cardSource)
    {
        return player.Creature != Owner || cardSource == null ? amount : amount + Amount;
    }

    public Task AfterModifyingConsumeCount(Player player, CardModel? cardSource)
    {
        Flash();
        return Task.CompletedTask;
    }
}