using Downfall.DownfallCode.CollectionFilters;
using Downfall.DownfallCode.Config;
using Downfall.DownfallCode.Nodes;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.PotionLab;
using MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection;

namespace Downfall.DownfallCode.Patches;

/// <summary>
/// Adds the filter sidebar (<see cref="NCollectionFilterPanel"/>) to the relic collection and the potion lab.
/// </summary>
[HarmonyPatch(typeof(NRelicCollection))]
public static class RelicCollectionFilterPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(NRelicCollection._Ready))]
    private static void AddPanel(NRelicCollection __instance) =>
        CollectionFilterAttach.Attach(__instance, RelicFilterSpec.Build);

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NRelicCollection.OnSubmenuOpened))]
    private static void ResetPanel(NRelicCollection __instance) => CollectionFilterAttach.Reset(__instance);
}

[HarmonyPatch(typeof(NPotionLab))]
public static class PotionLabFilterPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(NPotionLab._Ready))]
    private static void AddPanel(NPotionLab __instance) =>
        CollectionFilterAttach.Attach(__instance, PotionFilterSpec.Build);

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NPotionLab.OnSubmenuOpened))]
    private static void ResetPanel(NPotionLab __instance) => CollectionFilterAttach.Reset(__instance);
}

internal static class CollectionFilterAttach
{
    /// <summary>
    /// A failure here (missing scene, a game update moving something) must not take the vanilla screen's own
    /// <c>_Ready</c> down with it, so it is logged and the screen simply stays unfiltered.
    /// </summary>
    public static void Attach(Control screen, Func<CollectionFilterSpec> buildSpec)
    {
        if (!DownfallConfig.EnableCompendiumTools) return;

        try
        {
            var panel = NCollectionFilterPanel.Create(screen, buildSpec());
            screen.AddChild(panel);
            // Right behind the back button (declared after the screen contents), so the sidebar never covers it.
            screen.MoveChild(panel, screen.GetNode("%BackButton").GetIndex());
        }
        catch (Exception e)
        {
            DownfallMainFile.Logger.Error($"Could not add the filter panel to {screen.GetType().Name}: {e}");
        }
    }

    // The screens rebuild every entry on open, so stale selections would show an unfiltered list with filters lit.
    public static void Reset(Control screen) =>
        screen.GetNodeOrNull<NCollectionFilterPanel>(NCollectionFilterPanel.NodeName)?.Reset();
}
