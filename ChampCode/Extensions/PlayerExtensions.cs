using Champ.ChampCode.Core;
using Champ.ChampCode.Stance;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Champ.ChampCode.Extensions;

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
}