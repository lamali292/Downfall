using System.Reflection;
using Downfall.Code.Patches;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
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
        Harmony harmony = new(ModId);
        
        var assembly = Assembly.GetExecutingAssembly();
        ScriptManagerBridge.LookupScriptsInAssembly(assembly);
        harmony.PatchAll();
        
        DownfallSubscriber.Subscribe();
    }
}