using Downfall.Code.Core.Champ;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Events;

public interface IOnStanceChange
{
    Task OnStanceChange(PlayerChoiceContext ctx, Player player, StanceModel oldStance, StanceModel newStance);
}