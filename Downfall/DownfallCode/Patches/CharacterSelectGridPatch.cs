using BaseLib.BaseLibScenes;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Nodes.Screens.CustomRun;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(NCustomRunScreen), "InitCharacterButtons")]
internal static class CustomRunScreenScrollPatch
{
    [HarmonyPostfix]
    private static void MakeScrollable(NCustomRunScreen __instance)
    {
        var container = __instance.GetNodeOrNull<Control>("LeftContainer/CharSelectButtons/ButtonContainer");
        if (container == null) return;

        var buttons = container.GetChildren().OfType<NCharacterSelectButton>().ToList();
        if (buttons.Count <= 5) return;

        var parent = container.GetParent();
        var index = container.GetIndex();

        parent.RemoveChild(container);

        var scroll = NHorizontalScrollContainer.Create(
            "ButtonScrollContainer",
            container,
            c =>
            {
                c.AnchorLeft = 0.5f;
                c.AnchorTop = 0.5f;
                c.AnchorRight = 0.5f;
                c.AnchorBottom = 0.5f;
                c.OffsetLeft = -330f;
                c.OffsetTop = -177.0f;
                c.OffsetBottom = -10f;
                c.OffsetRight = 330f;
                c.GrowHorizontal = Control.GrowDirection.Both;
                c.GrowVertical = Control.GrowDirection.Both;
                c.ClipContents = true;
            });

        parent.AddChild(scroll);
        parent.MoveChild(scroll, index);
        scroll.AddChild(container);

        // Force position reset deferred so Godot layout doesn't override it
        container.AnchorLeft = 0;
        container.AnchorTop = 0;
        container.AnchorRight = 0;
        container.AnchorBottom = 0;
        container.SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin;
        container.CallDeferred(GodotObject.MethodName.Set, "position", Vector2.Zero);
    }
}

/*
[HarmonyPatch(typeof(NCustomRunScreen), nameof(NCustomRunScreen._Process))]
internal static class CustomRunScreenDebugPositionPatch
{
    private static float? _offsetY;
    private static float? _scrollOffsetTop;
    private static float? _scrollOffsetBottom;
    private static readonly float _step = 1f;
    private static readonly float _bigStep = 10f;

    [HarmonyPostfix]
    private static void AdjustPosition(NCustomRunScreen __instance)
    {
        var scroll = __instance.GetNodeOrNull<NHorizontalScrollContainer>("LeftContainer/CharSelectButtons/ButtonScrollContainer");
        if (scroll == null) return;

        var container = scroll.ScrollContents;
        if (container == null) return;

        _offsetY ??= container.Position.Y;
        _scrollOffsetTop ??= scroll.OffsetTop;
        _scrollOffsetBottom ??= scroll.OffsetBottom;

        var input = Input.Singleton;
        bool changed = false;
        float step = input.IsKeyPressed(Key.Shift) ? _bigStep : _step;

        if (input.IsKeyPressed(Key.Kp8)) { _offsetY -= step; changed = true; }
        if (input.IsKeyPressed(Key.Kp2)) { _offsetY += step; changed = true; }
        if (input.IsKeyPressed(Key.Kp7)) { _scrollOffsetTop -= step; changed = true; }
        if (input.IsKeyPressed(Key.Kp1)) { _scrollOffsetTop += step; changed = true; }
        if (input.IsKeyPressed(Key.Kp9)) { _scrollOffsetBottom -= step; changed = true; }
        if (input.IsKeyPressed(Key.Kp3)) { _scrollOffsetBottom += step; changed = true; }

        if (!changed) return;

        container.Position = container.Position with { Y = _offsetY.Value };
        scroll.OffsetTop = _scrollOffsetTop.Value;
        scroll.OffsetBottom = _scrollOffsetBottom.Value;

        GD.Print($"[DebugPos] ContentY={_offsetY} | OffsetTop={_scrollOffsetTop} OffsetBottom={_scrollOffsetBottom}");
    }
}
*/