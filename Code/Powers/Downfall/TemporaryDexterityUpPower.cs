using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Powers.Downfall;

public class TemporaryDexterityUpPower : TemporaryPower<DexterityPower>
{
    public override AbstractModel OriginModel => ModelDb.Power<DexterityPower>();
    protected override bool IsPositive => true;
}