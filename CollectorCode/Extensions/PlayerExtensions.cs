using BaseLib.Patches.Content;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Piles;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Extensions;

internal static class PlayerExtensions
{
    extension(Player player)
    {
        public IReadOnlyList<CardModel> CollectiblesPile =>
            CustomPiles.GetCustomPile(player.PlayerCombatState, CollectorPile.Collected)?.Cards
            ?? [];

        public int Essence => EssenceModel.GetEssence(player);

        public Creature? Torchhead()
        {
            return player.PlayerCombatState?.GetPet<TorchheadMonsterModel>();
        }

        public bool CanAffordEssence(int amount)
        {
            return EssenceModel.CanAfford(player, amount);
        }

        public void AddEssence(int amount)
        {
            EssenceModel.AddEssence(player, amount);
        }

        public bool SpendEssence(int amount)
        {
            return EssenceModel.SpendEssence(player, amount);
        }
    }
}