using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Hexaghost.HexaghostCode.Events;

public interface IModifyGhostflameEffectAdditive
{
    int ModifyGhostflameEffectAdditive(Player owner, GhostflameModel bolsteringGhostflame);
}