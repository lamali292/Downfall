using Downfall.DownfallCode.Config;
using MegaCrit.Sts2.Core.Helpers;

namespace Downfall.DownfallCode.Voting;

using Godot;
using HarmonyLib;
using Downfall.DownfallCode.Vfx;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;


[HarmonyPatch(typeof(NMainMenu), "_Ready")]
public class VotingMenuButton
{
    private static NMainMenuTextButton? _button;
    private static NMainMenuSubmenuStack? _stack;

    public static void Prefix(NMainMenu __instance)
    {
        _button = (NMainMenuTextButton)__instance.GetNode<NMainMenuTextButton>("MainMenuTextButtons/SettingsButton")
            .Duplicate();
    }

    public static void Postfix(NMainMenu __instance)
    {
        if (_button == null || !DownfallConfig.DevMode) return;
        __instance.GetNode<NMainMenuTextButton>("MainMenuTextButtons/SettingsButton").AddSibling(_button);
        _button.GetChild<MegaLabel>(0).Text = "Art Voting";
        _button.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(OnPress));

        // find and remember the submenu stack
        _stack = FindStack(__instance) ?? FindStack(__instance.GetTree().Root);

        if (VotingApi.Instance != null) return;
        var api = new VotingApi();
        __instance.GetTree().Root.AddChild(api); 
    }

    private static NMainMenuSubmenuStack? FindStack(Node root)
    {
        if (root is NMainMenuSubmenuStack s) return s;
        return root.GetChildren().Select(FindStack).OfType<NMainMenuSubmenuStack>().FirstOrDefault();
    }

    private static void OnPress(NButton button)
    {
        _stack?.PushSubmenuType<NArtVotingScreen>();
    }
}


[HarmonyPatch(typeof(NMainMenuSubmenuStack), nameof(NMainMenuSubmenuStack.GetSubmenuType), typeof(Type))]
public class VotingSubmenuPatch
{
    private static NArtVotingScreen? _cached;

    public static bool Prefix(Type type, NMainMenuSubmenuStack __instance, ref NSubmenu __result)
    {
        if (type != typeof(NArtVotingScreen))
            return true;
        if (_cached == null || !GodotObject.IsInstanceValid(_cached))
        {
            _cached = NArtVotingScreen.Create();
            if (_cached != null)
            {
                _cached.Visible = false;
                __instance.AddChildSafely(_cached);
            }
        }
        __result = _cached!;
        return false;
    }
}