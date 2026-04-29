using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Downfall.DownfallCode.Patches;

public interface IAdditionalOverlay
{
    string OverlayNodeName { get; }
    Control? CreateAdditionalOverlay();
}

[HarmonyPatch(typeof(NCard), nameof(NCard.ReloadOverlay))]
public static class CreateOverlayPatch
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

public interface IColoredPortrait
{
    float HueShift => 0f;
    float Saturation => 1f;
    float Value => 1f;
}

[HarmonyPatch(typeof(NCard), "Reload")]
public static class NCardReloadCollectiblePatch
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
