using System.Reflection;
using Champ.ChampCode.Cards;
using Champ.ChampCode.Events;
using Champ.ChampCode.Localization;
using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Patches;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace Champ.ChampCode;

[ModInitializer(nameof(Initialize))]
public partial class ChampMainFile : Node
{
    public const string ModId = "Champ"; //At the moment, this is used only for the Logger and harmony names.

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        CardDescriptionRegistry.Register<ChampCardModel>(DescriptionInjectionPoint.BelowMainText, new SkillBonusDescriptionSource());
        CardDescriptionRegistry.Register<ChampCardModel>(DescriptionInjectionPoint.BelowMainText, new FinisherDescriptionSource());
        ChampSubscriber.Subscribe();
        Harmony harmony = new(ModId);
        var assembly = Assembly.GetExecutingAssembly();
        ScriptManagerBridge.LookupScriptsInAssembly(assembly);
        harmony.PatchAll();
    }
}