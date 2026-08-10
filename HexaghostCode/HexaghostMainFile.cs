using BaseLib.Utils;
using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Patches;
using Downfall.DownfallCode.Utils;
using Godot;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using Hexaghost.HexaghostCode.Events;
using Hexaghost.HexaghostCode.Localization;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace Hexaghost.HexaghostCode;

[ModInitializer(nameof(Initialize))]
public partial class HexaghostMainFile : Node
{
    public const string ModId = "Hexaghost"; //At the moment, this is used only for the Logger and harmony names.

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        PostInitRegistry.Register(PostModelInit);
        RichTextEffectRegistry.Register<RichTextAfterlife>();
        CardExecutionRegistry.RegisterBefore(HexaghostCardEffectHandler.DoBeforeOnPlayInternal);
        CardExecutionRegistry.RegisterAfter(HexaghostCardEffectHandler.DoAfterOnPlayInternal);
        CustomLocTableManager.Register("ghostflames");
        HexaghostSubscriber.Subscribe();

        BundledSubmodLocRegistry.Register(ModId);
        CombatUiHooks.Register(HexaghostModel.SetupHexaghostCombatUi);
    }

    private static void PostModelInit()
    {
        CardKeywordSubRegistry.Register(CardKeyword.Ethereal, HexaghostKeyword.Afterlife);
        KeywordColorRegistry.Register(HexaghostKeyword.Afterlife, "afterlife");
    }
}