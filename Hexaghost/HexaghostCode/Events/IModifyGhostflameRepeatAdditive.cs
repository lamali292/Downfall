using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Hexaghost.HexaghostCode.Events;

public interface IModifyGhostflameRepeatAdditive
{
    int ModifyGhostflameRepeatAdditive(Player owner, GhostflameRepeatType repeatType,
        GhostflameModel bolsteringGhostflame);
}