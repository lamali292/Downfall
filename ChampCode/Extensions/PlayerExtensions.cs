using Champ.ChampCode.Core;
using Champ.ChampCode.Stance;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Champ.ChampCode.Extensions;

internal static class PlayerExtensions
{
    extension(Player player)
    {
        public ChampStanceModel ChampStance => ChampModel.GetStanceModel(player);

        public bool IsInChampStance<T>()
            where T : ChampStanceModel
        {
            return ChampModel.IsInStance<T>(player);
        }

        public bool ShouldDefensiveComboTrigger => ChampModel.IsInStance<ChampDefensiveStance>(player) ||
                                                     ChampModel.IsInStance<ChampUltimateStance>(player);

        public bool ShouldBerserkerComboTrigger => ChampModel.IsInStance<ChampBerserkerStance>(player) ||
                                                     ChampModel.IsInStance<ChampUltimateStance>(player);
    }
    
   
}