using Downfall.DownfallCode.Patches;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Extensions;

public static class PowerExtensions
{
    extension(PowerModel power)
    {
        public void InvokeSilentDisplayAmountChanged()
        {
            if (!UpdateAmountRegistry.RefreshActions.TryGetValue(power, out var action))
                return;
            action();
        }
    }
}