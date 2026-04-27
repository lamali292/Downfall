using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Collector.CollectorCode.Extensions;

internal static class PlayerExtensions
{
    public static Creature? Torchhead(this Player player)
    {
        return player.PlayerCombatState?.GetPet<TorchheadMonsterModel>();
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