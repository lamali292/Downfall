using System.Reflection;
using BaseLib.Extensions;
using BaseLib.Patches.Content;
using Downfall.DownfallCode.CustomEnums;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.CustomEnums;

/*
public class GuardianCardType
{
    [CustomEnum] public static CardType Gem;
}


[HarmonyPatch(typeof(ModelDb), "Init")]
public static class CardTypeGenerator
{
    [HarmonyAfter("alchyr.sts2.baselib")]
    [HarmonyPrefix]
    public static void FindAndGenerate()
    {
        CustomCardTypeRegistry.Register(GuardianCardType.Gem, new CardTypeProperties(CardType.Skill, CardType.Skill));
    }
}

[HarmonyPatch(typeof(CardModel), "get_PortraitBorderPath")]
public static class PortraitBorderPathPatch
{
    [HarmonyPrefix]
    private static bool Prefix(CardModel __instance, ref string __result)
    {
        var cardType = __instance.Type;
        if (cardType != GuardianCardType.Gem) return true;
        __result = CustomCardTypeRegistry.GetBorderPath(cardType);
        return false;
    }
}

[HarmonyPatch(typeof(CardModel), "get_FramePath")]
public static class FramePathPatch
{
    [HarmonyPrefix]
    private static bool Prefix(CardModel __instance, ref string __result)
    {
        var cardType = __instance.Type;
        if (cardType != GuardianCardType.Gem) return true;

        var path = CustomCardTypeRegistry.GetFramePath(cardType);
        __result = __instance.Rarity != CardRarity.Ancient
            ? ImageHelper.GetImagePath(path)
            : ImageHelper.GetImagePath("atlases/card_atlas.sprites/beta.tres");
        return false;
    }
}

[HarmonyPatch(typeof(CardTypeExtensions), nameof(CardTypeExtensions.ToLocString))]
public static class LogErrorPatch
{
    private static readonly Dictionary<CardType, string> NameCache = new();

    [HarmonyPrefix]
    public static bool Prefix(CardType cardType, ref LocString __result)
    {
        if (cardType != GuardianCardType.Gem) return true;
        __result = new LocString("gameplay_ui", GetInternalNameForCardType(cardType));
        return false;
    }

    private static string GetInternalNameForCardType(CardType targetValue)
    {
        if (NameCache.TryGetValue(targetValue, out var cached)) return cached;

        foreach (var t in ReflectionHelper.ModTypes)
        {
            var fields = t.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                if (field.FieldType != typeof(CardType)) continue;
                if (!Equals(field.GetValue(null), targetValue)) continue;

                var attr = field.GetCustomAttribute<CustomEnumAttribute>();
                var name = (attr?.Name ?? field.Name).ToUpperInvariant();
                var result = t.GetPrefix() + name;

                NameCache[targetValue] = result;
                return result;
            }
        }

        return "UNKNOWN_CARDTYPE";
    }
}
*/