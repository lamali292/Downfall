using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Abstract;

public abstract class CustomEnchantmentModel : EnchantmentModel, ICustomModel
{
    /// <summary>
    /// Override this or place your enchantment's image at
    /// "res://images/enchantments/modid-enchantment_name.png"
    /// </summary>
    protected virtual string? CustomIconPath => null;

    [HarmonyPatch(typeof(EnchantmentModel), nameof(IconPath), MethodType.Getter)]
    private static class IconPatch
    {
        private static bool Prefix(EnchantmentModel __instance, ref string __result)
        {
            if (__instance is not CustomEnchantmentModel customEnchantmentModel)
                return true;
            __result = customEnchantmentModel.CustomIconPath;
            return __result == null; 
        }
    }
}