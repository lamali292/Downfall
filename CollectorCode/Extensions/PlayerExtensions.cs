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
    public static Creature? Torchhead(this Player player)
    {
        return player.PlayerCombatState?.GetPet<TorchheadMonsterModel>();
    }

    
    public static IReadOnlyList<CardModel> GetCollectibles(this Player player)
    {
        return CustomPiles.GetCustomPile(player.PlayerCombatState, CollectorPile.Collected)?.Cards
               ?? [];
    }

    public static int GetEssence(this Player player)
    {
        return EssenceModel.GetEssence(player);
    }

    public static bool CanAffordEssence(this Player player, int amount)
    {
        return EssenceModel.CanAfford(player, amount);
    }

    public static void AddEssence(this Player player, int amount)
    {
        EssenceModel.AddEssence(player, amount);
    }

    public static bool SpendEssence(this Player player, int amount)
    {
        return EssenceModel.SpendEssence(player, amount);
    }
}