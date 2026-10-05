using Automaton.AutomatonCode.Cards;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Functions;
using Automaton.AutomatonCode.Localization;
using Automaton.AutomatonCode.Piles;
using BaseLib.Commands;
using BaseLib.Utils;
using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Patches;
using Downfall.DownfallCode.Utils;
using Downfall.DownfallCode.Voting;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace Automaton.AutomatonCode;

// Automaton is an internal submod (ADR 0003): its own assembly (Automaton.dll, built by
// Automaton.csproj) for code-separation, but NOT independently discovered/loaded by the game's mod
// loader - no [ModInitializer] here. Downfall's own MainFile loads this assembly by reflection and
// calls Initialize() directly (see DownfallMainFile.InitializeAutomaton()).
public static class AutomatonMainFile
{
    public const string ModId = "Automaton"; //At the moment, this is used only for the Logger and harmony names.

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(typeof(AutomatonMainFile).Assembly);

        // Before ModelDb.Init: the Function card takes its vars from this registry.
        EffectRegistry.RegisterAssembly(typeof(AutomatonMainFile).Assembly);
        CustomLocTableManager.Register("encode");
        CardDescriptionRegistry.Register<AutomatonCardModel>(DescriptionInjectionPoint.AboveMainText,
            new EncodeDescriptionSource());
        CardDescriptionRegistry.Register<AutomatonCardModel>(DescriptionInjectionPoint.BelowMainText,
            new CompileDescriptionSource());
        BundledSubmodLocRegistry.Register(ModId);
        VotingPoolRegistry.Register<AutomatonCardPool>(VotingPool.Automaton, ModId);
        FormBoneRegistry.RegisterVoidForm<Core.Automaton>("chest");
        FormBoneRegistry.RegisterSerpentForm<Core.Automaton>("chest");
        FormBoneRegistry.RegisterReaperForm<Core.Automaton>("chest");
        FormBoneRegistry.RegisterEchoForm<Core.Automaton>("chest");
        RegisterEncodeLocationFilter();
    }

    private static void RegisterEncodeLocationFilter()
    {
        CardPlayLocationCompat.RegisterInitialLocationFilter(EncodeOutcome.HideFromDiscard);
    }
    
}