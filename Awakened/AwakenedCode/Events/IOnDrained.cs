using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Events;

public interface IOnDrained
{
    Task OnDrained(PlayerChoiceContext ctx, Player player, int amount);
}