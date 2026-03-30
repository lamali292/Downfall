using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Extensions;

public static class DynamicVarExtensions
{
    // This allows you to do DynamicVars.Power<SplitWidePower>()
    public static DynamicVar Power<T>(this DynamicVarSet vars) where T : PowerModel
    {
        return vars[typeof(T).Name];
    }
}