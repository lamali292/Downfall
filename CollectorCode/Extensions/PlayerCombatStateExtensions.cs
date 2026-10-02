using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Collector.CollectorCode.Extensions;

public static class PlayerCombatStateExtensions
{
    extension(PlayerCombatState playerCombatState)
    {
        public int Reserve
        {
            get => CollectorEnergy.Instance?.Get(playerCombatState) ?? 0;
            set => CollectorEnergy.Instance?.Set(playerCombatState, value);
        } 
    }
}