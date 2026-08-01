using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Utils;
using Godot;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using Snecko.SneckoCode.Core;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace Snecko.SneckoCode;

[ModInitializer(nameof(Initialize))]
public partial class SneckoMainFile : Node
{
    public const string ModId = "Snecko"; //At the moment, this is used only for the Logger and harmony names.

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        CardExecutionRegistry.RegisterAfter(SneckoCardEffectHandler.DoAfterOnPlayInternal);
        BundledSubmodLocRegistry.Register(ModId);
        
        FormBoneRegistry.RegisterVoidForm<Core.Snecko>("eye");
        FormBoneRegistry.RegisterSerpentForm<Core.Snecko>("spine5");
        FormBoneRegistry.RegisterReaperForm<Core.Snecko>("spine10");
    }
}