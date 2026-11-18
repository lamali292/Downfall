using Downfall.DownfallCode.DynamicVars;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Extensions;

public static class DynamicVarsExtension
{
    extension(DynamicVarSet vars)
    {
        public EnemyDamageVar EnemyDamage
            => (EnemyDamageVar)vars["EnemyDamage"];

        public SelfDamageVar SelfDamage
            => (SelfDamageVar)vars["SelfDamage"];

        /// <summary>
        ///     The "HitCount" variable, whether it is the game's <c>HitCountVar</c> or our <c>DownfallHitCountVar</c>
        ///     (<c>DynamicVars.HitCount</c> only exists on newer game builds and casts to the game type).
        /// </summary>
        public DynamicVar HitCountCompat
            => vars[DownfallHitCountVar.DefaultName];

        public EnchantmentVar<T> Enchantment<T>() where T : EnchantmentModel
        {
            return (EnchantmentVar<T>)vars[typeof(T).Name];
        }
    }
}