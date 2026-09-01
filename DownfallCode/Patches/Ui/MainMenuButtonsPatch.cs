using Downfall.DownfallCode.Voting;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

namespace Downfall.DownfallCode.Patches;

// Lib mod
[HarmonyPatch(typeof(NMainMenu), "_Ready")]
internal static class MainMenuButtonsPatch
{
    private static NMainMenuSubmenuStack? _stack;

    [HarmonyPostfix]
    private static void Postfix(NMainMenu __instance)
    {
        var template = __instance.GetNode<NMainMenuTextButton>("MainMenuTextButtons/SettingsButton");
        _stack = FindStack(__instance) ?? FindStack(__instance.GetTree().Root);

        foreach (var entry in MainMenuButtonRegistry.Entries)
        {
            if (!entry.IsVisible()) continue;

            var button = (NMainMenuTextButton)template.Duplicate();
            template.AddSibling(button);
            button.GetChild<MegaLabel>(0).Text = entry.Label;

            var captured = entry;
            button.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(_ =>
            {
                if (captured.OnPress != null) captured.OnPress(_stack);
                else if (captured.SubmenuType != null) _stack?.PushSubmenuType(captured.SubmenuType);
            }));
        }
    }

    private static NMainMenuSubmenuStack? FindStack(Node root)
    {
        if (root is NMainMenuSubmenuStack s) return s;
        return root.GetChildren().Select(FindStack).OfType<NMainMenuSubmenuStack>().FirstOrDefault();
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