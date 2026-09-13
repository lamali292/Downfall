using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

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
    //Todo: Look into chemical X for modifying X.
}