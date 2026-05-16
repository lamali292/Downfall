using Downfall.DownfallCode.Powers.Abstract;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.DownfallCode.Powers;

public class TemporaryThornsPower : TemporaryPower<ThornsPower>
{
    public override AbstractModel OriginModel => ModelDb.Power<ThornsPower>();
    protected override bool IsPositive => true;
    protected override bool RemovedAfterOwnTurn => false;
}