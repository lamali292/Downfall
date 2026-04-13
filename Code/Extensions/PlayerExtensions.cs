using Downfall.Code.Commands;
using Downfall.Code.Core.Champ;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.Code.Extensions;

public static class PlayerExtensions
{
    public static ChampStanceModel ChampStance(this Player player)
    {
        return ChampModel.GetStanceModel(player);
    }
}