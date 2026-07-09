using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Patches;


// Lib mod
public static class CardTitleHooks
{
    // Takes (card, currentTitle), returns possibly-modified title
    private static readonly List<Func<CardModel, string, string>> modifiers = new();

    public static void Register(Func<CardModel, string, string> modifier)
        => modifiers.Add(modifier);

    internal static string ApplyModifiers(CardModel card, string title)
    {
        foreach (var modifier in modifiers)
        {
            try { title = modifier(card, title); }
            catch (Exception e) { DownfallMainFile.Logger.Error($"Title modifier failed: {e}"); }
        }
        return title;
    }
}


[HarmonyPatch(typeof(CardModel), nameof(CardModel.Title), MethodType.Getter)]
internal static class PatchCardTitle
{
    [HarmonyPostfix]
    private static void Postfix(CardModel __instance, ref string __result)
        => __result = CardTitleHooks.ApplyModifiers(__instance, __result);
}