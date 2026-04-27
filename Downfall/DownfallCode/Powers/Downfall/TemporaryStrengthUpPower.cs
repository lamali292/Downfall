using Downfall.DownfallCode.Powers.Abstract;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.DownfallCode.Powers.Downfall;

public class TemporaryStrengthUpPower : TemporaryPower<StrengthPower>
{
    public override AbstractModel OriginModel => ModelDb.Power<StrengthPower>();
    protected override bool IsPositive => true;
}