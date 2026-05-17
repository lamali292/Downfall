using System.Reflection;
using Automaton.AutomatonCode.Cards;
using Automaton.AutomatonCode.Localization;
using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Patches;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace Automaton.AutomatonCode;

[ModInitializer(nameof(Initialize))]
public partial class AutomatonMainFile : Node
{
    public const string ModId = "Automaton"; //At the moment, this is used only for the Logger and harmony names.

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        CardDescriptionRegistry.Register<AutomatonCardModel>(DescriptionInjectionPoint.AboveMainText,
            new EncodeDescriptionSource());
        CardDescriptionRegistry.Register<AutomatonCardModel>(DescriptionInjectionPoint.BelowMainText,
            new CompileDescriptionSource());
        CardDescriptionRegistry.Register<AutomatonCardModel>(DescriptionInjectionPoint.BelowMainText,
            new CompileErrorDescriptionSource());
        Harmony harmony = new(ModId);
        var assembly = Assembly.GetExecutingAssembly();
        ScriptManagerBridge.LookupScriptsInAssembly(assembly);
        harmony.PatchAll();
    }
}