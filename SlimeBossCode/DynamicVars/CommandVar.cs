using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace SlimeBoss.SlimeBossCode.DynamicVars;

public class CommandVar : DynamicVar
{
    public CommandVar(decimal value) : base("Command", value)
    {
        this.WithTooltip();
    }
}