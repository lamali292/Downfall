using Downfall.Code.Core.Hexaghost;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Events;

public interface IWheelMoved
{
    Task AfterWheelAdvance(PlayerChoiceContext ctx, Player player, CardModel? source, GhostflameModel ghostflame, int ghostflameIndex,
        bool silent);

    Task AfterWheelRetract(PlayerChoiceContext ctx, Player player, CardModel? source, GhostflameModel ghostflame, int ghostflameIndex,
        bool silent);
}