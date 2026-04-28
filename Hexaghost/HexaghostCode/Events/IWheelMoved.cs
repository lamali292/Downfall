using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Events;

public interface IWheelMoved
{
    Task AfterWheelAdvance(PlayerChoiceContext ctx, Player player, AbstractModel? source, GhostflameModel ghostflame, int ghostflameIndex,
        bool silent);

    Task AfterWheelRetract(PlayerChoiceContext ctx, Player player, AbstractModel? source, GhostflameModel ghostflame, int ghostflameIndex,
        bool silent);
}