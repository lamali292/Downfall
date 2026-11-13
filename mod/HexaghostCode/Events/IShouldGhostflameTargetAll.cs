using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Events;

public interface IShouldGhostflameTargetAll
{
    bool ShouldGhostflameTargetAll(GhostflameModel ghostflame, GhostflameRepeatType damage);
    Task AfterShouldGhostflameTargetedAll(PlayerChoiceContext ctx, GhostflameModel ghostflame);
}