using BaseLib.Patches.Content;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Extensions;

internal static class PlayerExtensions
{
    extension(Player player)
    {
        public Creature? Torchhead => player.PlayerCombatState?.GetPet<TorchheadMonsterModel>();
    }
}