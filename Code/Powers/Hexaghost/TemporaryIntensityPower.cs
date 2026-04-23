using Downfall.Code.Powers.Downfall;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Powers.Hexaghost;

public class TemporaryIntensityPower : TemporaryPower<IntensityPower>
{
    public override AbstractModel OriginModel => ModelDb.Power<IntensityPower>();
    protected override bool IsPositive => true;
}