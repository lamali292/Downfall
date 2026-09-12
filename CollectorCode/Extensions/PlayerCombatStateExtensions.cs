using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Collector.CollectorCode.Extensions;

public static class PlayerCombatStateExtensions
{
    extension(PlayerCombatState playerCombatState)
    {
        public int Reserve
        {
            get => CardResourceRegistry.Get<CollectorEnergy>()?.Get(playerCombatState) ?? 0;
            set => CardResourceRegistry.Get<CollectorEnergy>()?.Set(playerCombatState, value);
        } 
    }
}