using Automaton.AutomatonCode.DynamicVars;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Automaton.AutomatonCode.Extensions;

public static class DynamicVarsExtension
{
    extension(DynamicVarSet vars)
    {
        public StashVar Stash
            => (StashVar)vars._vars["Stash"];
    }
}