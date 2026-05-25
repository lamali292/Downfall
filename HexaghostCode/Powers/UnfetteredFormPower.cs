using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Hexaghost.HexaghostCode.Powers;

public class UnfetteredFormPower : HexaghostPowerModel, IModifyGhostflameRepeatAdditive
{
    public int ModifyGhostflameRepeatAdditive(Player owner, GhostflameRepeatType repeatType,
        GhostflameModel bolsteringGhostflame)
    {
        return owner.Creature != Owner ? 0 : Amount;
    }
}