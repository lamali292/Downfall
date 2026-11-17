using Downfall.DownfallCode.Interfaces;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Downfall.DownfallCode.Patches;

internal static class CustomPortraitApplier
{
    internal static void Apply(NCard nCard)
    {
        if (nCard.Model is not ICustomPortrait card) return;
        var portrait = nCard.GetNodeOrNull<TextureRect>("%Portrait");
        if (portrait == null) return;

        var texture = card.GetPortraitTexture();
        if (texture != null)
            portrait.Texture = texture;
    }
}

/// <summary>
///     The portrait assignment lives in UpdatePortrait(), which is called from both Reload()
///     and UpdateVisuals(). Patching it covers both paths (UpdateVisuals would otherwise stomp
///     our texture on every refresh). Method name is a string because the method is private.
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