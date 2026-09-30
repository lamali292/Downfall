using Collector.CollectorCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Core;

public static class ReserveCmd
{
    public static Task GainReserve(AbstractModel card)
    {
        return GainReserve(card.Player, card.DynamicVars.Reserve.IntValue);
    }
    
    public static Task GainReserve(Player player, int amount)
    {
        player.PlayerCombatState?.Reserve += amount;
        return Task.CompletedTask;
    }
}
