using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Hexaghost.HexaghostCode.Powers;

public class UnfetteredFormPower : HexaghostPowerModel, IModifyGhostflameRepeatAdditive
{
    public int ModifyGhostflameRepeatAdditive(Player owner, GhostflameRepeatType repeatType,
        GhostflameModel bolsteringGhostflame)
    {
        if (owner.Creature != Owner ||
            repeatType is not (GhostflameRepeatType.Damage or GhostflameRepeatType.Soulburn)) return 0;
        return Amount;
    }
}