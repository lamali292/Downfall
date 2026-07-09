using Downfall.DownfallCode.Interfaces;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(NCard), "UpdatePortrait")]
internal static class NCardPortraitPatch
{
    [HarmonyPostfix]
    private static void Postfix(NCard __instance)
    {
        if (__instance.Model is not ICustomPortrait card) return;

        var texture = card.GetPortraitTexture();
        if (texture != null)
            __instance._portrait.Texture = texture;
    }
}