using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace SlimeBoss.SlimeBossCode.DynamicVars;


public class SlurpVar : DynamicVar
{
    public SlurpVar(decimal value) : base("Slurp", value)
    {
        this.WithTooltip();
    }
}