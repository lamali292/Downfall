using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;

namespace Downfall.DownfallCode.Utils.UI;

public interface ITopBarElementDescriptor
{
    string ScenePath { get; }
    float Width { get; }
    bool CanUse(Player player);
}

public interface ITopBarElement
{
    void Initialize(Player player);
}

internal static class TopBarElementRegistry
{
    private static List<Type>? _types;
    internal static IReadOnlyList<Type> Types => _types ??= Discover();

    private static List<Type> Discover()
    {
        var results = new List<Type>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            IEnumerable<Type> types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null)!;
            }

            results.AddRange(types.Where(t =>
                t is { IsClass: true, IsAbstract: false } &&
                t.IsAssignableTo(typeof(ITopBarElementDescriptor))));
        }

        return results;
    }
}

[HarmonyPatch(typeof(NTopBar), nameof(NTopBar.Initialize))]
internal class PatchTopBarInitialize
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