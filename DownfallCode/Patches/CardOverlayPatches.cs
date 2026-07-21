using Downfall.DownfallCode.Interfaces;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(NCard))]
internal static class CardOverlayPatches
{
    private const string NodeName = "_card_overlay_";

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NCard.Reload))]
    private static void ReloadPostfix(NCard __instance)
    {
        Sync(__instance);
    }

    internal static void Sync(NCard ncard)
    {
        var existing = ncard.GetNodeOrNull<Control>(NodeName);

        if (ncard.Model is not ICardOverlay provider)
        {
            // Reused node, model no longer wants an overlay → remove stale one
            existing?.QueueFree();
            return;
        }

        if (existing == null)
        {
            existing = provider.CreateCustomOverlay();
            existing.Name = NodeName;
            existing.MouseFilter = Control.MouseFilterEnum.Ignore;
            ncard.AddChildSafely(existing);
        }

        provider.UpdateOverlay(existing);
    }
}