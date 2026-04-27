using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Events;

public interface IAfterGhostwheelAllIgnited
{
    Task AfterGhostwheelAllIgnited(PlayerChoiceContext ctx, Player player, GhostflameModel flame, int index);
}