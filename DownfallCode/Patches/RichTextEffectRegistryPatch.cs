using Downfall.DownfallCode.Utils;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(MegaRichTextLabel), nameof(MegaRichTextLabel.InstallEffectsIfNeeded))]
public static class RichTextEffectRegistryPatch
{
    [HarmonyPostfix]
    public static void Postfix(MegaRichTextLabel __instance)
        => RichTextEffectRegistry.InstallInto(__instance);
}