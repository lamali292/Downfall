using Downfall.Code.Core.Champ;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Events;

public interface IOnChampStanceChange
{
    Task OnChampStanceChange(PlayerChoiceContext ctx, Player player, ChampStanceModel oldStance,
        ChampStanceModel newStance);
}