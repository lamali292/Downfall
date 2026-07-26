using Downfall.DownfallCode.Patches;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Extensions;

public static class PowerExtensions
{
    public static void InvokeSilentDisplayAmountChanged(this PowerModel power)
    {
        if (!UpdateAmountRegistry.RefreshActions.TryGetValue(power, out var action))
            return;
        action();
    }
}
