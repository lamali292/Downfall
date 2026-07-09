using Hermit.HermitCode.Core;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Hermit.HermitCode.Relics;

/// <summary>
///     Spyglass
/// </summary>
public sealed class Spyglass : HermitRelicModel
{
    public Spyglass() : base(RelicRarity.Uncommon)
    {
        WithTip<ConcentrationPower>();
    }
}