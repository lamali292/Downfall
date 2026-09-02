using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SlimeBoss.SlimeBossCode.DynamicVars;

namespace SlimeBoss.SlimeBossCode.Extensions;

public static class DynamicVarsExtension
{
    extension(DynamicVarSet vars)
    {
        public SlimeSecondaryVar Slime => (SlimeSecondaryVar)vars._vars["Slime"];
    }
}