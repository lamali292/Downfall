using Downfall.DownfallCode.DynamicVars;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Downfall.DownfallCode.Extensions;

public static class DynamicVarsExtension
{
    extension(DynamicVarSet vars)
    {
        public EnemyDamageVar EnemyDamage
            => (EnemyDamageVar)vars._vars["EnemyDamage"];

        public SelfDamageVar SelfDamage
            => (SelfDamageVar)vars._vars["SelfDamage"];
    }
}