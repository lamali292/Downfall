using System.Reflection;
using System.Runtime.CompilerServices;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;

namespace Downfall.Code.Utils.UI;

// ── Interface ─────────────────────────────────────────────────────────────────

/// <summary>
/// Implemented by any node that should be automatically added to the top bar.
/// The patch discovers all concrete implementors via reflection — no manual
/// registration required.
/// </summary>
public interface ITopBarElement
{
    /// <summary>res:// path to the PackedScene for this element.</summary>
    string ScenePath { get; }

    /// <summary>Whether this element should appear for the given player.</summary>
    Func<Player, bool> CanUse { get; }

    /// <summary>
    /// Width reserved in the top bar spacer for this element.
    /// Typically matches the horizontal gap between elements (e.g. 80f).
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
            try { types = assembly.GetTypes(); }
            catch (ReflectionTypeLoadException ex) { types = ex.Types.Where(t => t != null)!; }

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

// ── Patch ─────────────────────────────────────────────────────────────────────

[HarmonyPatch(typeof(NTopBar), nameof(NTopBar.Initialize))]
class PatchTopBarInitialize
{
    [HarmonyPostfix]
    static void AddRegisteredElements(NTopBar __instance, IRunState runState)
    {
        var localPlayer = LocalContext.GetMe(runState);
        if (localPlayer == null) return;

        var elements = new List<(Control node, ITopBarElement element)>();

        foreach (var type in TopBarElementRegistry.Types)
        {
            var (scenePath, canUse, _) = TopBarElementRegistry.ReadMetadata(type);
            if (!canUse(localPlayer)) continue;

            var scene = ResourceLoader.Load<PackedScene>(scenePath);
            if (scene == null) continue;

            var node = scene.Instantiate<Control>();
            var element = (ITopBarElement)node;
            __instance.AddChildSafely(node);
            element.Initialize(localPlayer);
            elements.Add((node, element));
        }

        if (elements.Count == 0) return;

        Callable.From(() =>
        {
            // Stack elements leftward from the Map button.
            var cursor = __instance.Map.GlobalPosition;
            var totalWidth = 0f;

            foreach (var (node, element) in elements)
            {
                cursor += new Vector2(-element.Width, 0);
                node.GlobalPosition = cursor;
                totalWidth += element.Width;
            }

            // Insert a spacer so the map node's siblings don't overlap.
            var map = __instance.GetNodeOrNull<Node>("RightAlignedStuff/Map");
            if (map == null) return;

            var spacer = new Control();
            spacer.CustomMinimumSize = new Vector2(totalWidth, 0);
            spacer.MouseFilter = Control.MouseFilterEnum.Ignore;
            var parent = map.GetParent();
            parent.AddChild(spacer);
            parent.MoveChild(spacer, map.GetIndex());
        }).CallDeferred();
    }
}