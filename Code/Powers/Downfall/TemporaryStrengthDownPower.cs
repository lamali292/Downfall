using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Powers.Downfall;

public class TemporaryStrengthDownPower : TemporaryPower<StrengthPower>
{
    public override AbstractModel OriginModel => ModelDb.Power<StrengthPower>();
    protected override bool IsPositive => false;
}