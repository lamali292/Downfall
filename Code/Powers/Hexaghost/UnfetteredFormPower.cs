using Downfall.Code.Abstract;
using Downfall.Code.Core.Hexaghost;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.Code.Powers.Hexaghost;

public class UnfetteredFormPower : HexaghostPowerModel, IModifyGhostflameRepeatAdditive
{
    public int ModifyGhostflameRepeatAdditive(Player owner, GhostflameRepeatType repeatType, GhostflameModel bolsteringGhostflame)
    {
        if (owner.Creature != Owner || repeatType is not (GhostflameRepeatType.Damage or GhostflameRepeatType.Soulburn)) return 0;
        return Amount;
    }
}