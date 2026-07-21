using Automaton.AutomatonCode.Cards;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Localization;
using BaseLib.Utils;
using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Patches;
using Downfall.DownfallCode.Utils;
using Godot;
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
        CustomLocTableManager.Register("encode");
        CardExecutionRegistry.RegisterBefore(AutomatonCardEffectHandler.DoBeforeOnPlayInternal);
        CardExecutionRegistry.RegisterAfter(AutomatonCardEffectHandler.DoAfterOnPlayInternal);
        CardDescriptionRegistry.Register<AutomatonCardModel>(DescriptionInjectionPoint.AboveMainText,
            new EncodeDescriptionSource());
        BundledSubmodLocRegistry.Register(ModId);
    }
}