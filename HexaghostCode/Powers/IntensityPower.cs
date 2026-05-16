using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Hexaghost.HexaghostCode.Powers;

public class IntensityPower : HexaghostPowerModel, IModifyGhostflameEffectAdditive
{
    public int ModifyGhostflameEffectAdditive(Player owner, GhostflameModel bolsteringGhostflame)
    {
        return Amount;
    }
}