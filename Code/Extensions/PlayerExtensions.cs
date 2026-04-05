using Downfall.Code.Core.Champ;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.Code.Extensions;

public static class PlayerExtensions
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
        return ChampModel.IsInStance<DefensiveChampStance>(player) ||
               ChampModel.IsInStance<UltimateChampStance>(player);
    }

    public static bool ShouldBerserkerComboTrigger(this Player player)
    {
        return ChampModel.IsInStance<BerserkerChampStance>(player) ||
               ChampModel.IsInStance<UltimateChampStance>(player);
    }
}