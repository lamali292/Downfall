using Downfall.DownfallCode.Interfaces;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(NCard), "Reload")]
public static class CardColorPatch
{
    public static void Postfix(NCard __instance)
    {
        if (__instance.Model is not IColoredPortrait collectible) return;

        var portrait = __instance.GetNodeOrNull<TextureRect>("%Portrait");
        if (portrait == null) return;

        var shaderMaterial = new ShaderMaterial();
        shaderMaterial.Shader = ResourceLoader.Load<Shader>("res://shaders/hsv.gdshader");
        shaderMaterial.SetShaderParameter("h", collectible.HueShift);
        shaderMaterial.SetShaderParameter("s", collectible.Saturation);
        shaderMaterial.SetShaderParameter("v", collectible.Value);
        portrait.Material = shaderMaterial;
    }
}