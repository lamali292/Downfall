using Downfall.DownfallCode.Utils.UI;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(NTopBar), nameof(NTopBar.Initialize))]
internal class TopBarInitializePatch
{
    [HarmonyPostfix]
    private static void AddRegisteredElements(NTopBar __instance, IRunState runState)
    {
        var localPlayer = LocalContext.GetMe(runState);
        if (localPlayer == null) return;

        var rightContainer = __instance.GetNodeOrNull<HBoxContainer>("RightAlignedStuff");
        if (rightContainer == null) return;

        foreach (var type in TopBarElementRegistry.Types)
        {
            var descriptor = (ITopBarElementDescriptor)Activator.CreateInstance(type)!;
            if (!descriptor.CanUse(localPlayer)) continue;

            var scene = ResourceLoader.Load<PackedScene>(descriptor.ScenePath);
            if (scene == null) continue;

            var node = scene.Instantiate<Control>();
            node.CustomMinimumSize = new Vector2(descriptor.Width, 0);
            node.SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin;

            rightContainer.AddChild(node);
            rightContainer.MoveChild(node, 3);

            if (node is ITopBarElement element)
                element.Initialize(localPlayer);
        }
    }
}