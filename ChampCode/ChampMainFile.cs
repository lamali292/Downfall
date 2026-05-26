using System.Reflection;
using BaseLib.Utils;
using Champ.ChampCode.Cards;
using Champ.ChampCode.Events;
using Champ.ChampCode.Localization;
using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Patches;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace Champ.ChampCode;

[ModInitializer(nameof(Initialize))]
public partial class ChampMainFile : Node
{
    public const string ModId = "Champ"; //At the moment, this is used only for the Logger and harmony names.

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        CustomLocTableManager.Register("champ_stances");
        CardDescriptionRegistry.Register<ChampCardModel>(DescriptionInjectionPoint.BelowMainText,
            new SkillBonusDescriptionSource());
        CardDescriptionRegistry.Register<ChampCardModel>(DescriptionInjectionPoint.BelowMainText,
            new FinisherDescriptionSource());
        ChampSubscriber.Subscribe();
        Harmony harmony = new(ModId);
        var assembly = Assembly.GetExecutingAssembly();
        ScriptManagerBridge.LookupScriptsInAssembly(assembly);
        harmony.PatchAll();
    }
}