using Downfall.Code.Core;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Events;

public interface IOnStanceChange
{
    Task OnStanceChange(PlayerChoiceContext ctx, Player player, ChampStance oldStance, ChampStance newStance);
}