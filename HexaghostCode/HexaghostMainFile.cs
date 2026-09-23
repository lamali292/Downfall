using BaseLib.Utils;
using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Patches;
using Downfall.DownfallCode.Utils;
using Downfall.DownfallCode.Voting;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using Hexaghost.HexaghostCode.Events;
using Hexaghost.HexaghostCode.Localization;
using Hexaghost.HexaghostCode.Patches;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace Hexaghost.HexaghostCode;

[ModInitializer(nameof(Initialize))]
public static class HexaghostMainFile
{
    public const string ModId = "Hexaghost"; //At the moment, this is used only for the Logger and harmony names.

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        PostInitRegistry.Register(PostModelInit);
        RichTextEffectRegistry.Register<RichTextAfterlife>();
        CardExecutionHooks.RegisterBefore(HexaghostCardEffectHandler.DoBeforeOnPlayInternal);
        CardExecutionHooks.RegisterAfter(HexaghostCardEffectHandler.DoAfterOnPlayInternal);
        CustomLocTableManager.Register("ghostflames");
        HexaghostSubscriber.Subscribe();

        BundledSubmodLocRegistry.Register(ModId);
        VotingPoolRegistry.Register<HexaghostCardPool>(VotingPool.Hexaghost);

        ModPatcher.Create(ModId, Logger)
            .Add(typeof(NCreatureAnimationPatch))
            .Add(typeof(PatchCreatureHoverTips))
            .PatchAll();
    }

    private static void PostModelInit()
    {
        CardKeywordSubRegistry.Register(CardKeyword.Ethereal, HexaghostKeyword.Afterlife);
        KeywordColorRegistry.Register(HexaghostKeyword.Afterlife, "afterlife");
    }
}