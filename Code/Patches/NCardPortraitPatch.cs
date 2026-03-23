using Downfall.Code.Cards.Automaton.Token;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Downfall.Code.Patches;

[HarmonyPatch(typeof(NCard))]
[HarmonyPatch("Reload")]
public static class NCardPortraitPatch
{
    private static void Postfix(NCard __instance)
    {
        if (__instance.Model is not FunctionCard fc) return;

        var composite = fc.GetCompositePortrait();
        if (composite == null) return;

        var portrait = __instance.GetNode<TextureRect>("%Portrait");
        if (portrait != null)
            portrait.Texture = composite;
    }
}