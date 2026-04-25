using Downfall.Code.Abstract;
using Downfall.Code.Core.Hexaghost;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.Code.Powers.Hexaghost;

public class IntensityPower : HexaghostPowerModel, IModifyGhostflameEffectAdditive
{
    public int ModifyGhostflameEffectAdditive(Player owner, GhostflameModel bolsteringGhostflame)
    {
        return Amount;
    }
}