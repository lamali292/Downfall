using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Hermit.HermitCode.Powers;

public class BigBruiserPower : HermitPowerModel
{
    public BigBruiserPower() : base(PowerType.Buff, PowerStackType.Single)
    {
        WithTip<BruisePower>();
    }
}