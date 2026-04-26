using Downfall.Code.Core.Hexaghost;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.Code.Events;

public interface IModifyGhostflameRepeatAdditive
{
    int ModifyGhostflameRepeatAdditive(Player owner, GhostflameRepeatType repeatType, GhostflameModel bolsteringGhostflame);
}