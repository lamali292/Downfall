using System.Reflection;
using BaseLib.Config;
using Downfall.Code.Config;
using Downfall.Code.Events;
using Downfall.Code.Localization;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using SmartFormat;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace Downfall.Code;

[ModInitializer(nameof(Initialize))]
public partial class DownfallMainFile : Node
{
    public const string ModId = "Downfall";

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);


    public static void Initialize()
    {
        ModConfigRegistry.Register(ModId, new DownfallConfig());
        Harmony harmony = new(ModId);

        var assembly = Assembly.GetExecutingAssembly();
        ScriptManagerBridge.LookupScriptsInAssembly(assembly);
        harmony.PatchAll();

        Smart.Default.AddExtensions(new PowerIconFormatter());
        DownfallSubscriber.Subscribe();
    }
}



[HarmonyPatch(typeof(Log), nameof(Log.Error))]
public static class LogErrorPatch
{
    [HarmonyPrefix]
    public static bool DowngradeLocErrors(string text)
    {
        return !text.StartsWith("Localization formatting error!");
    }
}