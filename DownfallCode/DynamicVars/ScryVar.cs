using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Downfall.DownfallCode.DynamicVars;

public class ScryVar : DynamicVar
{
    public ScryVar(decimal value) : base("Scry", value)
    {
        this.WithTooltip();
    }
}