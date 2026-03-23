using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.Code.Extensions;

namespace Downfall.Code.Powers.Automaton;

public abstract class AutomatonPowerModel : CustomPowerModel
{
    private string IconName => Id.Entry
        .RemovePrefix()
        .RemoveSuffix("Power")
        .ToLowerInvariant();

    public override string CustomPackedIconPath => $"{IconName}.png".PowerImageAutomatonPath();
    public override string CustomBigIconPath => $"{IconName}.png".BigPowerImageAutomatonPath();
}