using Downfall.DownfallCode.Interfaces;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Downfall.DownfallCode.Patches;

internal static class CustomPortraitApplier
{
    internal static void Apply(NCard nCard)
    {
        if (nCard.Model is not ICustomPortrait card) return;
        if (nCard._portrait == null) return;

        var texture = card.GetPortraitTexture();
        if (texture != null)
            nCard._portrait.Texture = texture;
    }
}

/// <summary>
///     Stable branch: the portrait is assigned inline inside Reload(),
///     and Reload is the only place it's written. Patching Reload covers everything.
/// </summary>
[HarmonyPatch(typeof(NCard), nameof(NCard.Reload))]
internal static class NCardReloadPortraitPatch
{
    [HarmonyPostfix]
    private static void Postfix(NCard __instance)
    {
        CustomPortraitApplier.Apply(__instance);
    }
}

/// <summary>
///     Beta branch: portrait assignment was refactored into UpdatePortrait(),
///     which is called from both Reload() and UpdateVisuals(). Patching it covers
///     both paths (UpdateVisuals would otherwise stomp our texture on every refresh).
///     Method name is a string on purpose — it doesn't exist on stable.
/// </summary>
[HarmonyPatch(typeof(NCard), "UpdatePortrait")]
internal static class NCardUpdatePortraitPatch
{
    [HarmonyPostfix]
    private static void Postfix(NCard __instance)
    {
        CustomPortraitApplier.Apply(__instance);
    }
}