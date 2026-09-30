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
using MegaCrit.Sts2.Core.Modding;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace Automaton.AutomatonCode;

[ModInitializer(nameof(Initialize))]
public static class AutomatonMainFile
{
    public const string ModId = "Automaton"; //At the moment, this is used only for the Logger and harmony names.

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        // Before ModelDb.Init: the Function card takes its vars from this registry.
        EffectRegistry.RegisterAssembly(typeof(AutomatonMainFile).Assembly);
        PostInitRegistry.Register(PostModelInit);
        CustomLocTableManager.Register("encode");
        CardExecutionHooks.RegisterBefore(AutomatonCardEffectHandler.DoBeforeOnPlayInternal);
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

    private static void PostModelInit()
    {
        // todo: use actual stash pile icon.
        MultiPileCardSelect.RegisterPileIndicator(StashPile.Stash,
            "res://Automaton/images/character/character_icon.png",
            new LocString("card_selection", "AUTOMATON-STASH_PILE"));
    }
}