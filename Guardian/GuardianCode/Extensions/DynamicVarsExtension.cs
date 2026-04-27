using Guardian.GuardianCode.DynamicVars;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guardian.GuardianCode.Extensions;

public static class DynamicVarsExtension
{
    public static BraceVar Brace(this DynamicVarSet vard)
    {
        return (BraceVar)vard._vars[nameof(Brace)];
    }

    public static AccelerateVar Accelerate(this DynamicVarSet vard)
    {
        return (AccelerateVar)vard._vars[nameof(Accelerate)];
    }
}