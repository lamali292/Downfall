using Collector.CollectorCode.Core;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;

namespace Collector.CollectorCode.Patches;

[HarmonyPatch(typeof(NCardLibrary), nameof(NCardLibrary._Ready))]
internal static class AddMyPoolFilterPatch
{
    
    private static Texture2D IconTexture => PreloadManager.Cache.GetTexture2D(IconTexturePath);
    private static string IconTexturePath => "res://Collector/scenes/ui/collector_pile.png";


    [HarmonyPostfix]
    [HarmonyPriority(Priority.Low)]
    private static void Postfix(NCardLibrary __instance)
    {

        var myPool = ModelDb.CardPool<CollectibleCardPool>();
        var icon = IconTexture;
        if (__instance._miscPoolFilter.GetParentControl() is not { } parentControl)
            return;

        var filter = BuildFilterIcon(__instance, icon);
        parentControl.AddChild(filter);
        filter.Owner = parentControl.Owner;

        __instance._poolFilters.Add(filter, c => myPool.AllCardIds.Contains(c.Id));
        filter.Connect(NCardPoolFilter.SignalName.Toggled,
            Callable.From<NCardPoolFilter>(__instance.UpdateCardPoolFilter));
        filter.Connect(Control.SignalName.FocusEntered,
            Callable.From(() => __instance._lastHoveredControl = filter));
    }

    private static NCardPoolFilter BuildFilterIcon(NCardLibrary instance, Texture2D iconTexture)
    {
        var source = instance._miscPoolFilter;
        var filter = (NCardPoolFilter)source.Duplicate();
        filter.Name = "FILTER-COLLECTOR-COLLECTIBLES";
        foreach (var rect in filter.FindChildren("*", nameof(TextureRect), true, false)
                                   .OfType<TextureRect>())
        {
            rect.Texture = iconTexture;
        }

        return filter;
    }
}