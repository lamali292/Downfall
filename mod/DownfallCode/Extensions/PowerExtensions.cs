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
        
        
        public PowerType DownfallType => power.GetDownfallTypeForAmount(power.Amount);
        public PowerType GetDownfallTypeForAmount(decimal customAmount)
        {
            if (customAmount < 0 && power is ThornsPower) return PowerType.Debuff;
            return power.GetTypeForAmount(customAmount);
        }

        /// <summary>
        ///     For use in <c>AfterPowerAmountChanged</c>: this power was just created on an enemy as a debuff,
        ///     i.e. the enemy did not have it before (the caller checks who applied it). A temporary power
        ///     wrapper (Flashbang, ...) is skipped: it applies the real power (Strength) nested inside itself,
        ///     which is the one that counts, so the debuff is not counted twice.
        /// </summary>
        public bool IsNewEnemyDebuff(decimal appliedAmount)
        {
            return power.Owner is { IsEnemy: true }
                   && power is not ITemporaryPower
                   && power.Amount == appliedAmount && power.DownfallType == PowerType.Debuff;
        }
    }
}