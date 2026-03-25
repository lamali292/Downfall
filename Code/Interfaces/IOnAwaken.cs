using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Interfaces;

public interface IOnAwaken
{
    Task OnAwaken(PlayerChoiceContext ctx, Player player)
        => Task.CompletedTask;
}