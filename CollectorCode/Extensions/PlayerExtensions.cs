using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Collector.CollectorCode.Extensions;

internal static class PlayerExtensions
{
    extension(Player player)
    {
        public Creature? Torchhead => player.PlayerCombatState?.GetPet<TorchheadMonsterModel>();
        /// <summary>Is Osty present in combat and alive?</summary>
        public bool IsTorchheadAlive
        {
            get
            {
                var torchhead =player.Torchhead;
                return torchhead != null && torchhead.IsAlive;
            }
        }

        /// <summary>Has Torchhead been summoned but is at 0 HP? It still attacks, at half damage.</summary>
        public bool IsTorchheadDowned => player.Torchhead is { IsAlive: false };

        /// <summary>Is Torchhead missing from combat or dead?</summary>
        public bool IsTorchheadMissing => !player.IsTorchheadAlive;
    }
}