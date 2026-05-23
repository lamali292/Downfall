using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Automaton.AutomatonCode.DynamicVars;

public class StashVar : DynamicVar
{
    public StashVar(decimal value) : base("Stash", value)
    {
        this.WithTooltip();
    }
}