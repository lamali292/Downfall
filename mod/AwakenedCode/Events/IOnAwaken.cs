using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Events;

public interface IOnAwaken
{
    Task OnAwaken(PlayerChoiceContext ctx, Player player);
}