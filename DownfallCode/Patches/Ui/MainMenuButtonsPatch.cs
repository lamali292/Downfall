using Downfall.DownfallCode.Voting;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

namespace Downfall.DownfallCode.Patches;

// Lib mod
//
// Modded buttons used to be duplicated into the vanilla "MainMenuTextButtons" VBoxContainer,
// which is anchored to a fixed-height box (~450px) already nearly filled by the 8 vanilla
// buttons. Every extra registered entry pushed the list past that box and overflowed the
// screen. Instead, like RitsuLib's mod-settings shortcut, we give modded buttons their own
// free-standing container anchored to an empty corner of the main menu, independent of the
// vanilla list.
[HarmonyPatch(typeof(NMainMenu), "_Ready")]
internal static class MainMenuButtonsPatch
{
    internal const string ContainerNodeName = "DownfallModMenuButtons";

    private static NMainMenuSubmenuStack? _stack;

    [HarmonyPostfix]
    private static void Postfix(NMainMenu __instance)
    {
        var template = __instance.GetNode<NMainMenuTextButton>("MainMenuTextButtons/SettingsButton");
        _stack = FindStack(__instance) ?? FindStack(__instance.GetTree().Root);

        var container = new VBoxContainer
        {
            Name = ContainerNodeName,
            AnchorLeft = 0f,
            AnchorTop = 1f,
            AnchorRight = 0f,
            AnchorBottom = 1f,
            OffsetLeft = 18f,
            OffsetBottom = -16f,
            GrowHorizontal = Control.GrowDirection.End,
            GrowVertical = Control.GrowDirection.Begin,
            Alignment = BoxContainer.AlignmentMode.End,
        };
        __instance.AddChildSafely(container);

        foreach (var entry in MainMenuButtonRegistry.Entries)
        {
            if (!entry.IsVisible()) continue;

            var button = (NMainMenuTextButton)template.Duplicate();
            button.CustomMinimumSize = new Vector2(240f, 56f);
            container.AddChild(button);

            var label = button.GetChild<MegaLabel>(0);
            // The vanilla label is a tiny anchor box that free-grows around its center to fit
            // one short word ("Continue", "Quit"...) - fine for vanilla text, but a longer
            // localized label ("Downfall Art Submission") would grow symmetrically off the
            // left edge of the screen. Anchor it to the button's actual rect instead so
            // wrap/auto-shrink have a real box to fit into.
            label.AnchorLeft = 0f;
            label.AnchorTop = 0f;
            label.AnchorRight = 1f;
            label.AnchorBottom = 1f;
            label.OffsetLeft = 0f;
            label.OffsetTop = 0f;
            label.OffsetRight = 0f;
            label.OffsetBottom = 0f;
            label.AutowrapMode = TextServer.AutowrapMode.WordSmart;
            label.AutoSizeEnabled = true;
            label.MinFontSize = 14;
            label.MaxFontSize = 32;
            label.SetTextAutoSize(entry.GetDisplayText());

            var captured = entry;
            button.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(_ =>
            {
                if (captured.OnPress != null) captured.OnPress(_stack);
                else if (captured.SubmenuType != null) _stack?.PushSubmenuType(captured.SubmenuType);
            }));
        }

        MainMenuButtonRegistry.InvokeMainMenuReady();
    }

    private static NMainMenuSubmenuStack? FindStack(Node root)
    {
        if (root is NMainMenuSubmenuStack s) return s;
        return root.GetChildren().Select(FindStack).OfType<NMainMenuSubmenuStack>().FirstOrDefault();
    }
}

// Vanilla toggles "MainMenuTextButtons"/PatchNotesButton visibility here whenever a submenu
// opens/closes (e.g. leaving the main menu screen). Our free-standing container isn't part of
// that vanilla node, so without this it stays visible on top of submenus.
[HarmonyPatch(typeof(NMainMenu), "OnSubmenuStackChanged")]
internal static class MainMenuButtonsVisibilityPatch
{
    [HarmonyPostfix]
    private static void Postfix(NMainMenu __instance)
    {
        if (__instance.GetNodeOrNull<Control>(MainMenuButtonsPatch.ContainerNodeName) is { } container)
            container.Visible = !__instance.SubmenuStack.SubmenusOpen;
    }
}

[HarmonyPatch(typeof(NMainMenuSubmenuStack), nameof(NMainMenuSubmenuStack.GetSubmenuType), typeof(Type))]
internal static class CustomSubmenuPatch
{
    private static readonly Dictionary<Type, NSubmenu> cache = new();

    [HarmonyPrefix]
    private static bool Prefix(Type type, NMainMenuSubmenuStack __instance, ref NSubmenu __result)
    {
        var entry = MainMenuButtonRegistry.FindBySubmenuType(type);
        if (entry?.CreateSubmenu == null) return true; // not ours → run original

        if (!cache.TryGetValue(type, out var menu) || !GodotObject.IsInstanceValid(menu))
        {
            menu = entry.CreateSubmenu();
            if (menu == null) return true;
            menu.Visible = false;
            __instance.AddChildSafely(menu);
            cache[type] = menu;
        }

        __result = menu;
        return false;
    }
}