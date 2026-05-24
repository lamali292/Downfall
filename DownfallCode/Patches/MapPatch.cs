/*
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;

namespace Downfall.DownfallCode.Patches;

// 1. Fix base auto scaling (replaces NGlobalUi's clamped logic)
[HarmonyPatch(typeof(NGlobalUi), "OnWindowChange")]
public static class NGlobalUiWindowChangePatch
{
    private static bool Prefix(Window ____window)
    {
        if (SaveManager.Instance.SettingsSave.AspectRatioSetting != AspectRatioSetting.Auto)
            return true;

        ____window.ContentScaleAspect = Window.ContentScaleAspectEnum.Expand;
        ____window.ContentScaleSize = new Vector2I(1680, 1080);
        return false;
    }
}

// 2. On map open, zoom out to show full map
[HarmonyPatch(typeof(NMapScreen), nameof(NMapScreen.Open))]
public static class NMapScreenOpenPatch
{
    private static void Postfix(NMapScreen __instance)
    {
        var window = NGame.Instance?.GetTree().Root;
        if (window == null) return;

        window.ContentScaleAspect = Window.ContentScaleAspectEnum.Expand;
        window.ContentScaleSize = new Vector2I(1680, 2500);

        var mapContainer = __instance.GetNode<Control>("TheMap");
        if (mapContainer != null)
            mapContainer.Position = new Vector2(0f, 1800f);

        var mapBg = __instance.GetNode<NMapBg>("%MapBg");
        if (mapBg != null)
        {
            // normal: map=-600, bg=-1620 → offset=-1020
            // full: map=1800, bg=1800-1020=780
            mapBg.Position = new Vector2(mapBg.Position.X, 780f);
        }
    }
}

[HarmonyPatch(typeof(NMapScreen), "UpdateScrollPosition")]
public static class NMapScreenScrollPatch
{
    private static bool Prefix(NMapScreen __instance)
    {
        if (!__instance.IsOpen) return true;

        var window = NGame.Instance?.GetTree().Root;
        if (window == null) return true;
        if (window.ContentScaleSize.Y <= 1080) return true;

        var mapContainer = __instance.GetNode<Control>("TheMap");
        if (mapContainer != null)
            mapContainer.Position = new Vector2(0f, 1800f);

        var mapBg = __instance.GetNode<NMapBg>("%MapBg");
        if (mapBg != null)
            mapBg.Position = new Vector2(mapBg.Position.X, 780f);

        return false;
    }
}

[HarmonyPatch(typeof(NMapScreen), nameof(NMapScreen.Close))]
public static class NMapScreenClosePatch
{
    private static void Prefix(NMapScreen __instance)
    {
        if (!__instance.IsOpen) return;

        var window = NGame.Instance?.GetTree().Root;
        if (window == null) return;

        window.ContentScaleAspect = Window.ContentScaleAspectEnum.Expand;
        window.ContentScaleSize = new Vector2I(1680, 1080);

        // Let NMapBg.OnWindowChange recalculate its own position on close
        var mapBg = __instance.GetNode<NMapBg>("%MapBg");
        mapBg?.EmitSignal(Viewport.SignalName.SizeChanged);
    }
}

// Prevent NMapBg from fighting our position override while map is open
[HarmonyPatch(typeof(NMapBg), "OnWindowChange")]
public static class NMapBgWindowChangePatch
{
    private static bool Prefix(NMapBg __instance)
    {
        var window = NGame.Instance?.GetTree().Root;
        if (window == null) return true;
        if (window.ContentScaleSize.Y <= 1080) return true;

        // Block NMapBg from overriding our position while in full map mode
        return false;
    }
}
*/