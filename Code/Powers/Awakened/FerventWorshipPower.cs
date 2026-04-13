using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Downfall.Code.Powers.Awakened;

public class FerventWorshipPower : AwakenedPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}