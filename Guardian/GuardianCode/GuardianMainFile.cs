using System.Reflection;
using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Patches;
using Godot;
using Godot.Bridge;
using Guardian.GuardianCode.Cards;
using Guardian.GuardianCode.Localization;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace Guardian.GuardianCode;

[ModInitializer(nameof(Initialize))]
public partial class GuardianMainFile : Node
{
    public const string ModId = "Guardian"; //At the moment, this is used only for the Logger and harmony names.

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        CardDescriptionRegistry.Register<GuardianCardModel>(DescriptionInjectionPoint.BelowMainText, new GemDescriptionSource());
        _ = GuardianCardModel.GemData;
        Harmony harmony = new(ModId);
        var assembly = Assembly.GetExecutingAssembly();
        ScriptManagerBridge.LookupScriptsInAssembly(assembly);
        harmony.PatchAll();
    }
}


