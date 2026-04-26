using Downfall.Code.Core.Hexaghost;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Events;

public interface IAfterGhostwheelIgnited
{
    Task AfterGhostwheelIgnited(PlayerChoiceContext ctx, Player player, GhostflameModel flame, int index);
}