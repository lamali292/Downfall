using System.Reflection;
using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Patches;
using Godot;
using Godot.Bridge;
using Guardian.GuardianCode.Cards;
using Guardian.GuardianCode.Events;
using Guardian.GuardianCode.Localization;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace Guardian.GuardianCode;

[ModInitializer(nameof(Initialize))]
public partial class GuardianMainFile : Node
{
    public const string ModId = "Guardian";

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        CardDescriptionRegistry.Register<GuardianCardModel>(DescriptionInjectionPoint.BelowMainText,
            new GemDescriptionSource());
        _ = GuardianCardModel.GemData;
        Harmony harmony = new(ModId);
        GuardianSubsriber.Subscribe();
        var assembly = Assembly.GetExecutingAssembly();
        ScriptManagerBridge.LookupScriptsInAssembly(assembly);
        harmony.PatchAll();
    }
}