using Downfall.DownfallCode.Interfaces;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(NPower), "_Ready")]
internal static class CustomPowerIconPatch
{
    private const string DecorPrefix = "_custom_icon_";

    [HarmonyPostfix]
    private static void Postfix(NPower __instance)
    {
        if (__instance._model is not ICustomPowerIcon power) return;
        power.IconChanged += () => Refresh(__instance);
        Refresh(__instance);
    }

    private static void Refresh(NPower instance)
    {
        if (!GodotObject.IsInstanceValid(instance)) return;
        if (instance._model is not ICustomPowerIcon power) return;

        var icon = instance.GetNode<TextureRect>("%Icon");

        foreach (var child in icon.GetChildren())
            if (child.Name.ToString().StartsWith(DecorPrefix))
                child.QueueFree();

        power.DecorateIcon(icon);
    }
}