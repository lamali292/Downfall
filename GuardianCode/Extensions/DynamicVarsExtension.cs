using Guardian.GuardianCode.DynamicVars;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guardian.GuardianCode.Extensions;

public static class DynamicVarsExtension
{
    extension(DynamicVarSet vars)
    {
        public BraceVar Brace => (BraceVar)vars._vars["Brace"];

        public AccelerateVar Accelerate => (AccelerateVar)vars._vars["Accelerate"];

        public PolishVar Polish => (PolishVar)vars._vars["Polish"];

        public GemVar Gem => (GemVar)vars._vars["Gem"];
    }
}