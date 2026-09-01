using Downfall.DownfallCode.Interfaces;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(NCard), nameof(NCard.ReloadOverlay))]
public static class CardOverlayPatch
{
    [HarmonyPostfix]
    public static void CreatureOverlay(NCard __instance)
    {
        foreach (var child in __instance._overlayContainer.GetChildren())
        {
            if (!child.Name.ToString().StartsWith("Downfall")) continue;
            child.Name = "DELETING_OLD_OVERLAY";
            child.QueueFreeSafely();
        }

        if (__instance.Model is not IAdditionalOverlay additional) return;
        var customNode = additional.CreateAdditionalOverlay();
        if (customNode == null) return;
        customNode.Name = additional.OverlayNodeName;
        __instance._overlayContainer.AddChildSafely(customNode);
    }
}