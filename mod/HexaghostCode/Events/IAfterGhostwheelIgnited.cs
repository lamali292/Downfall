using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Events;

public interface IAfterGhostflameIgnited
{
    Task AfterGhostflameIgnited(PlayerChoiceContext ctx, Player player, GhostflameModel flame, int index);
}