using Automaton.AutomatonCode.Piles;
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
        public Creature? Torchhead() => player.PlayerCombatState?.GetPet<TorchheadMonsterModel>();

        public IReadOnlyList<CardModel> CollectiblesPile =>
            CustomPiles.GetCustomPile(player.PlayerCombatState, CollectorPile.Collected)?.Cards
            ?? [];

        public int Essence => EssenceModel.GetEssence(player);

        public bool CanAffordEssence(int amount) => EssenceModel.CanAfford(player, amount);

        public void AddEssence(int amount) => EssenceModel.AddEssence(player, amount);

        public bool SpendEssence(int amount) => EssenceModel.SpendEssence(player, amount);
    }
}