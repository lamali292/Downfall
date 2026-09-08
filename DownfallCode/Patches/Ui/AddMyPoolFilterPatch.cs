using BaseLib.Patches.UI;
using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;

namespace Downfall.DownfallCode.Patches;


[HarmonyPatch(typeof(NCardLibrary), nameof(NCardLibrary._Ready))]
internal static class AddMyPoolFilterPatch
{
   
    [HarmonyPostfix]
    [HarmonyPriority(Priority.Low)]
    private static void Postfix(NCardLibrary __instance)
    {
        var myPool = ModelDb.CardPool<CollectibleCardPool>();
        var icon = ModelDb.Character<Collector.CollectorCode.Core.Collector>().IconTexture;
        if (__instance._miscPoolFilter.GetParentControl() is not { } parentControl)
            return;
 
        var filter = BuildFilterIcon(icon);
        parentControl.AddChild(filter);
        filter.Owner = parentControl.Owner;
 
        __instance._poolFilters.Add(filter, c => myPool.AllCardIds.Contains(c.Id));
        filter.Connect(NCardPoolFilter.SignalName.Toggled,
            Callable.From<NCardPoolFilter>(__instance.UpdateCardPoolFilter));
        filter.Connect(Control.SignalName.FocusEntered,
            Callable.From(() => __instance._lastHoveredControl = filter));
    }
 
    private static NCardPoolFilter BuildFilterIcon(Texture2D iconTexture)
    {
        var filter = new NCardPoolFilter
        {
            Name = "FILTER-MyPool",
            Size = new Vector2(64f, 64f),
            CustomMinimumSize = new Vector2(64f, 64f),
            FocusMode = Control.FocusModeEnum.All
        };
 
        var image = new TextureRect
        {
            Name = "Image",
            Texture = iconTexture,
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            Size = new Vector2(56f, 56f),
            Position = new Vector2(4f, 4f),
            Scale = new Vector2(0.9f, 0.9f),
            PivotOffset = new Vector2(28f, 28f),
            Material = ShaderUtils.GenerateHsv(1f, 1f, 1f)
        };
 
        var shadow = new TextureRect
        {
            Name = "Shadow",
            Texture = iconTexture,
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            Size = new Vector2(56f, 56f),
            Position = new Vector2(4f, 3f),
            PivotOffset = new Vector2(28f, 28f),
            ShowBehindParent = true,
            Modulate = Colors.Black with { A = 0.25f }
        };
        image.AddChild(shadow);
 
        var reticle = PreloadManager.Cache
            .GetScene(SceneHelper.GetScenePath("ui/selection_reticle"))
            .Instantiate<NSelectionReticle>();
        reticle.Name = "SelectionReticle";
        reticle.UniqueNameInOwner = true;
 
        filter.AddChild(image);
        image.Owner = filter;
        filter.AddChild(reticle);
        reticle.Owner = filter;
 
        return filter;
    }
}
