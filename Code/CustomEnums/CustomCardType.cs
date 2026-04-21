using System.Reflection;
using BaseLib.Extensions;
using BaseLib.Patches.Content;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.CustomEnums;

public static class CustomCardType
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
        CustomCardTypeRegistry.Register(CustomCardType.Gem, new CardTypeProperties(CardType.Skill, CardType.Skill));
    }
}

public static class CustomCardTypeRegistry
{
    private static readonly Dictionary<CardType, CardTypeProperties> Properties = new();

    public static string GetFramePath(CardType type)
    {
        return Properties[type].FramePath.Path;
    }

    public static string GetBorderPath(CardType type)
    {
        return Properties[type].BorderPath.Path;
    }

    public static void Register(CardType type, CardTypeProperties properties)
    {
        Properties[type] = properties;
    }
}

public record CardTypeProperties(CardBorderPath BorderPath, FramePath FramePath);

public class CardBorderPath(string path)
{
    public string Path { get; } = path;

    public static implicit operator CardBorderPath(string text)
    {
        return new CardBorderPath(text);
    }

    public static implicit operator CardBorderPath(CardType keyword)
    {
        return new CardBorderPath(ImageHelper.GetImagePath(
            $"atlases/ui_atlas.sprites/card/card_portrait_border_{keyword.ToString().ToLowerInvariant()}_s.tres"));
    }
}

public class FramePath(string path)
{
    public string Path { get; } = path;

    public static implicit operator FramePath(string text)
    {
        return new FramePath(text);
    }

    public static implicit operator FramePath(CardType keyword)
    {
        return new FramePath(
            $"atlases/ui_atlas.sprites/card/card_frame_{keyword.ToString().ToLowerInvariant()}_s.tres"
        );
    }
}

[HarmonyPatch(typeof(CardModel), "get_PortraitBorderPath")]
public static class PortraitBorderPathPatch
{
    [HarmonyPrefix]
    private static bool Prefix(CardModel __instance, ref string __result)
    {
        var cardType = __instance.Type;
        if (cardType != CustomCardType.Gem) return true;
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
        if (cardType != CustomCardType.Gem) return true;

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
        if (cardType != CustomCardType.Gem) return true;
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

                NameCache[targetValue] = result; // Save it!
                return result;
            }
        }

        return "UNKNOWN_CARDTYPE";
    }
}