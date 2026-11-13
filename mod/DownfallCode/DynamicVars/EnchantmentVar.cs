using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.DynamicVars;

public class EnchantmentVar<T> : DynamicVar where T : EnchantmentModel
{
    public EnchantmentVar(decimal powerAmount)
        : base(typeof(T).Name, powerAmount)
    {
    }

    public EnchantmentVar(string name, decimal powerAmount)
        : base(name, powerAmount)
    {
    }
}