using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.Code.Extensions;

namespace Downfall.Code.Powers.Guardian;

public abstract class GuardianPowerModel : CustomPowerModel
{
    private string IconName => Id.Entry
        .RemovePrefix()
        .RemoveSuffix("Power")
        .ToLowerInvariant();

    public override string CustomPackedIconPath => $"{IconName}.png".PowerImageGuardianPath();
    public override string CustomBigIconPath => $"{IconName}.png".BigPowerImageGuardianPath();
}