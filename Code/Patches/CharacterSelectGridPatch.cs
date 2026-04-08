using System.Reflection;
using System.Reflection.Emit;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Nodes.Screens.CustomRun;

namespace Downfall.Code.Patches;

[HarmonyPatch]
internal static class CustomRunScreenPagedPatch
{
    private const int PageSize = 5;

    private static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(NCustomRunScreen), "InitCharacterButtons");
    }

    [HarmonyPostfix]
    private static void AddPaging(NCustomRunScreen __instance)
    {
        var container = __instance.GetNodeOrNull<Control>("LeftContainer/CharSelectButtons/ButtonContainer");
        if (container == null) return;
        container.OffsetLeft += 60f;
        var buttons = container.GetChildren().OfType<NCharacterSelectButton>().ToList();
        if (buttons.Count <= PageSize) return;

        var buttonSize = buttons[0].CustomMinimumSize;
        if (buttonSize == Vector2.Zero)
            buttonSize = new Vector2(150, 200); // fallback

        var prevWrapper = CreateArrowWrapper(true, buttonSize);
        var nextWrapper = CreateArrowWrapper(false, buttonSize);

        var prevBtn = prevWrapper.GetChild<NGoldArrowButton>(0);
        var nextBtn = nextWrapper.GetChild<NGoldArrowButton>(0);

        container.AddChildSafely(prevWrapper);
        container.MoveChild(prevWrapper, 0);
        container.AddChildSafely(nextWrapper);
        container.MoveChild(nextWrapper, buttons.Count + 1);

        var placeholders = new List<Control>();
        for (var i = 0; i < PageSize; i++)
        {
            var placeholder = new Control();
            placeholder.CustomMinimumSize = buttons[0].CustomMinimumSize;
            placeholder.Visible = false;
            container.AddChildSafely(placeholder);
            placeholders.Add(placeholder);
        }

        container.AddChildSafely(prevBtn);
        container.MoveChild(prevBtn, 0);

        container.AddChildSafely(nextBtn);
        var nextIndex = buttons.Count + 1;
        container.MoveChild(nextBtn, nextIndex);

        var page = 0;
        var totalPages = (int)Math.Ceiling(buttons.Count / (float)PageSize);

        prevBtn.Connect(NClickableControl.SignalName.Released,
            Callable.From<NButton>(_ =>
            {
                if (page > 0) ShowPage(page - 1);
            }));
        nextBtn.Connect(NClickableControl.SignalName.Released,
            Callable.From<NButton>(_ =>
            {
                if (page < totalPages - 1) ShowPage(page + 1);
            }));

        ShowPage(0);
        return;

        void ShowPage(int p)
        {
            page = Math.Clamp(p, 0, totalPages - 1);
            var start = page * PageSize;
            var end = start + PageSize;
            var visibleCount = 0;
            for (var i = 0; i < buttons.Count; i++)
            {
                var visible = i >= start && i < end;
                buttons[i].Visible = visible;
                if (visible) visibleCount++;
            }

            var needed = PageSize - visibleCount;
            for (var i = 0; i < placeholders.Count; i++)
                placeholders[i].Visible = i < needed;

            prevBtn.Modulate = page > 0 ? Colors.White : new Color(1, 1, 1, 0.3f);
            nextBtn.Modulate = page < totalPages - 1 ? Colors.White : new Color(1, 1, 1, 0.3f);
        }
    }

    private static Control CreateArrowWrapper(bool isLeft, Vector2 buttonSize)
    {
        var wrapper = new Control();
        wrapper.CustomMinimumSize = buttonSize;

        var arrow = new NGoldArrowButton();
        arrow.AnchorLeft = 0.5f;
        arrow.AnchorRight = 0.5f;
        arrow.AnchorTop = 0.5f;
        arrow.AnchorBottom = 0.5f;
        arrow.OffsetLeft = -40f;
        arrow.OffsetRight = 40f;
        arrow.OffsetTop = -32f;
        arrow.OffsetBottom = 32f;
        arrow.GrowHorizontal = Control.GrowDirection.Both;
        arrow.GrowVertical = Control.GrowDirection.Both;

        var shader = ResourceLoader.Load<Shader>("res://shaders/hsv.gdshader");
        var mat = new ShaderMaterial();
        mat.Shader = shader;
        mat.SetShaderParameter("h", 1.0f);
        mat.SetShaderParameter("s", 1.0f);
        mat.SetShaderParameter("v", 1.0f);

        var texturePath = isLeft
            ? "res://images/atlases/ui_atlas.sprites/settings_tiny_left_arrow.tres"
            : "res://images/atlases/ui_atlas.sprites/settings_tiny_right_arrow.tres";

        var icon = new TextureRect
        {
            Material = mat,
            Texture = ResourceLoader.Load<Texture2D>(texturePath),
            CustomMinimumSize = new Vector2(64, 64),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Name = (StringName)"TextureRect"
        };

        arrow.AddChild(icon);
        wrapper.AddChild(arrow);
        return wrapper;
    }
}

/**
 * changes
 * this._charButtonContainer.GetChild
 * <NCharacterSelectButton>
 *     (0)
 *     with
 *     this._charButtonContainer.GetChildren().OfType<NCharacterSelectButton>().First()
 */
[HarmonyPatch(typeof(NCustomRunScreen), nameof(NCustomRunScreen.OnSubmenuOpened))]
internal static class CustomRunOnSubmenuOpenedPatch
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> FixGetChild(IEnumerable<CodeInstruction> instructions)
    {
        var list = instructions.ToList();
        for (var i = 0; i < list.Count - 2; i++)
        {
            if (list[i].opcode != OpCodes.Ldc_I4_0 ||
                list[i + 1].opcode != OpCodes.Ldc_I4_0 ||
                list[i + 2].opcode != OpCodes.Callvirt ||
                list[i + 2].operand is not MethodInfo { Name: nameof(Node.GetChild), IsGenericMethod: true } m ||
                m.GetGenericArguments()[0] != typeof(NCharacterSelectButton)) continue;
            list[i] = new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(CustomRunOnSubmenuOpenedPatch), nameof(GetFirstCharButton)));
            list.RemoveAt(i + 2);
            list.RemoveAt(i + 1);
            break;
        }

        return list;
    }

    private static NCharacterSelectButton GetFirstCharButton(Control container)
    {
        return container.GetChildren().OfType<NCharacterSelectButton>().First();
    }
}