using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Utils;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using SlimeBoss.SlimeBossCode.Patches;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace Collector.CollectorCode;

[ModInitializer(nameof(Initialize))]
public static class CollectorMainFile
{
    public const string ModId = "Collector"; //At the moment, this is used only for the Logger and harmony names.

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        HivePowerExemptRegistry.Register<TorchheadMonsterModel>();
        CardExecutionRegistry.RegisterBefore(CollectorCardEffectHandler.DoBeforeOnPlayInternal);

        BundledSubmodLocRegistry.Register(ModId);
        
        FormBoneRegistry.RegisterVoidForm<Core.Collector>("robeback");
        FormBoneRegistry.RegisterSerpentForm<Core.Collector>("robeback");
        FormBoneRegistry.RegisterReaperForm<Core.Collector>("robeback");
        FormBoneRegistry.RegisterEchoForm<Core.Collector>("robeback");
    }
}