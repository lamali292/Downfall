using System.Reflection;
using System.Runtime.CompilerServices;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;

namespace Downfall.DownfallCode.Utils.UI;

// ── Interface ─────────────────────────────────────────────────────────────────

/// <summary>
///     Implemented by any node that should be automatically added to the top bar.
///     The patch discovers all concrete implementors via reflection — no manual
///     registration required.
/// </summary>
public interface ITopBarElement
{
    /// <summary>res:// path to the PackedScene for this element.</summary>
    string ScenePath { get; }

    /// <summary>Whether this element should appear for the given player.</summary>
    Func<Player, bool> CanUse { get; }

    /// <summary>
    ///     Width reserved in the top bar spacer for this element.
    ///     Typically matches the horizontal gap between elements (e.g. 80f).
    /// </summary>
    float Width { get; }

    /// <summary>Called after the element is added to the scene.</summary>
    void Initialize(Player player);
}

// ── Registry ──────────────────────────────────────────────────────────────────

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
                t.IsAssignableTo(typeof(ITopBarElement))));
        }

        return results;
    }

    internal static (string scenePath, Func<Player, bool> canUse, float width) ReadMetadata(Type type)
    {
        var probe = (ITopBarElement)RuntimeHelpers.GetUninitializedObject(type);
        return (probe.ScenePath, probe.CanUse, probe.Width);
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
            var (scenePath, canUse, width) = TopBarElementRegistry.ReadMetadata(type);
            if (!canUse(localPlayer)) continue;

            var scene = ResourceLoader.Load<PackedScene>(scenePath);
            if (scene == null) continue;

            var node = scene.Instantiate<Control>();
            node.CustomMinimumSize = new Vector2(width, 0);
            node.SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin;

            rightContainer.AddChild(node);
            rightContainer.MoveChild(node, 3);
            if (node is ITopBarElement element) element.Initialize(localPlayer);
        }
    }
}