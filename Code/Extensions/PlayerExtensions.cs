using Downfall.Code.Core.Champ;
using Downfall.Code.Core.Collector;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.Code.Extensions;

internal static class PlayerExtensions
{
    public static ChampStanceModel ChampStance(this Player player)
    {
        return ChampModel.GetStanceModel(player);
    }

    public static bool IsInChampStance<T>(this Player player)
        where T : ChampStanceModel
    {
        return ChampModel.IsInStance<T>(player);
    }

    public static bool ShouldDefensiveComboTrigger(this Player player)
    {
        return ChampModel.IsInStance<ChampDefensiveStance>(player) ||
               ChampModel.IsInStance<ChampUltimateStance>(player);
    }

    public static bool ShouldBerserkerComboTrigger(this Player player)
    {
        return ChampModel.IsInStance<ChampBerserkerStance>(player) ||
               ChampModel.IsInStance<ChampUltimateStance>(player);
    }

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