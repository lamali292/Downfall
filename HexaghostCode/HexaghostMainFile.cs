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
using MegaCrit.Sts2.Core.Modding;

namespace Hexaghost.HexaghostCode;

[ModInitializer(nameof(Initialize))]
public partial class HexaghostMainFile : Node
{
    public const string ModId = "Hexaghost"; //At the moment, this is used only for the Logger and harmony names.

    //public static Logger Logger { get; } =new(ModId, LogType.Generic);

    public static void Initialize()
    {
        RichTextEffectRegistry.Register<RichTextAfterlife>();
        CustomLocTableManager.Register("ghostflames");
        HexaghostSubscriber.Subscribe();

        BundledSubmodLocRegistry.Register(ModId);

        PostInitRegistry.Register(() =>
        {
            CardKeywordSubRegistry.Register(CardKeyword.Ethereal, HexaghostKeyword.Afterlife);
            KeywordColorRegistry.Register(HexaghostKeyword.Afterlife, "afterlife");
        });

        CombatUiHooks.Register(HexaghostModel.SetupHexaghostCombatUi);
    }
}