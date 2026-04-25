using Downfall.Code.Core.Hexaghost;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Events;

public interface IWheelMoved
{
    Task AfterWheelAdvance(PlayerChoiceContext ctx, Player player, GhostflameModel ghostflame, int ghostflameIndex,
        bool silent);

    Task AfterWheelRetract(PlayerChoiceContext ctx, Player player, GhostflameModel ghostflame, int ghostflameIndex,
        bool silent);
}