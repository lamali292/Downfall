using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Downfall.DownfallCode.Extensions;

public static class DynamicVarExtension
{
    public static decimal Calculate(this DynamicVar var, Creature? target)
    {
        return var is CalculatedVar calculatedVar ? calculatedVar.Calculate(target) : 0;
    }
}