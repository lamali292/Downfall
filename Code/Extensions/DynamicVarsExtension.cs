using Downfall.Code.DynamicVars;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Downfall.Code.Extensions;

public static class DynamicVarsExtension
{
    public static BraceVar Brace(this DynamicVarSet vard)
            => (BraceVar) vard._vars[nameof (Brace)];
    
    public static AccelerateVar Accelerate(this DynamicVarSet vard)
        => (AccelerateVar) vard._vars[nameof (Accelerate)];
}

