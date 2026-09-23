using Downfall.DownfallCode.Patches;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

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
        
        
        public PowerType DownfallType => power.GetTypeForAmount(power.Amount);
        public PowerType GetDownfallTypeForAmount(decimal customAmount)
        {
            if (customAmount < 0 && power is ThornsPower) return PowerType.Debuff;
            return power.GetTypeForAmount(customAmount);
        }
    }
}