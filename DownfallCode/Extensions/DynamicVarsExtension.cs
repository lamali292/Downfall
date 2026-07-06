using Downfall.DownfallCode.DynamicVars;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Downfall.DownfallCode.Extensions;

public static class DynamicVarsExtension
{
    public static EnemyDamageVar EnemyDamage(this DynamicVarSet vard)
    {
        return (EnemyDamageVar)vard._vars[nameof(EnemyDamage)];
    }

    public static SelfDamageVar SelfDamage(this DynamicVarSet vard)
    {
        return (SelfDamageVar)vard._vars[nameof(SelfDamage)];
    }
}