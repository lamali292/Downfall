using Downfall.DownfallCode.Interfaces;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Downfall.DownfallCode.Patches;

// modifier to plaques. something like "Skill | Gem" to mark guardian gems or "Power | Slime" to mark slime powers
[HarmonyPatch(typeof(NCard), nameof(NCard.UpdateTypePlaque))]
public static class NCardUpdateTypePlaquePatch
{
    private static LocString PlaqueLocString => new("gameplay_ui", "DOWNFALL-PLAQUE");
    public static void Postfix(NCard __instance)
    {
        var model = __instance.Model;
        if (model is not ICustomTypePlaque customPlaque) return;
        var originalText = model.Type.ToLocString().GetFormattedText();
        var overrideText = customPlaque.GetTypePlaqueName.GetFormattedText();
        if (string.IsNullOrEmpty(overrideText)) return;
        var text = PlaqueLocString;
        text.Add("original",  originalText);
        text.Add("type",  overrideText);
        var result = text.GetFormattedText();
        __instance._typeLabel.SetTextAutoSize(result);
        Callable.From(__instance.UpdateTypePlaqueSizeAndPosition).CallDeferred();
    }
}