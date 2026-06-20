using System.Reflection;
using Downfall.DownfallCode.Utils;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;


namespace Hermit.HermitCode;

[ModInitializer(nameof(Initialize))]
public partial class HermitMainFile : Node
{
    public const string ModId = "Hermit";

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        CardExecutionRegistry.RegisterBefore(HermitCardEffectHandler.DoBeforeOnPlayInternal);
        CardExecutionRegistry.RegisterAfter(HermitCardEffectHandler.DoAfterOnPlayInternal);
        Harmony harmony = new(ModId);
        var assembly = Assembly.GetExecutingAssembly();
        ScriptManagerBridge.LookupScriptsInAssembly(assembly);
        harmony.PatchAll();
    }
}