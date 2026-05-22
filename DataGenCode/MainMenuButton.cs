using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

namespace DataGen.DataGenCode;

[HarmonyPatch(typeof(NMainMenu), "_Ready")]
public class MainMenuButton
{
    private static NMainMenuTextButton? _button;

    public static void Prefix(NMainMenu __instance)
    {
        _button = (NMainMenuTextButton)__instance.GetNode<NMainMenuTextButton>("MainMenuTextButtons/SettingsButton")
            .Duplicate();
    }

    public static void Postfix(NMainMenu __instance)
    {
        if (_button == null) return;
        __instance.GetNode<NMainMenuTextButton>("MainMenuTextButtons/SettingsButton").AddSibling(_button);
        ViewportManager.AddToTree(_button.GetTree());
        _button.GetChild<MegaLabel>(0).Text = "Exporter";
        _button.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(OnPress));
    }

    private static void OnPress(NButton button)
    {
        if (NGame.Instance == null) return;
        NGame.Instance.AddChild(new ExporterScreen());
        //new ExportBatch().Run();
    }
}