using Downfall.DownfallCode.Powers.Abstract;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Powers;

public class TemporaryIntensityPower : TemporaryPower<IntensityPower, Core.Hexaghost>
{
    public override AbstractModel OriginModel => ModelDb.Power<IntensityPower>();
    protected override bool IsPositive => true;
}