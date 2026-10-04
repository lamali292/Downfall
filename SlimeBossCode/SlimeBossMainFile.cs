using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Utils;
using Downfall.DownfallCode.Voting;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using SlimeBoss.SlimeBossCode.Patches;
using SlimeBoss.SlimeBossCode.Slimes;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace SlimeBoss.SlimeBossCode;

[ModInitializer(nameof(Initialize))]
public static class SlimeBossMainFile
{
    public const string ModId = "SlimeBoss"; //At the moment, this is used only for the Logger and harmony names.

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        // Submod supersession guard (ADR 0003): "SlimeBossBeta" doesn't exist yet - this is the
        // forward-declared replacement ModId a future SlimeBoss Beta standalone submod will use.
        // If it's ever loaded alongside this bundled SlimeBoss, skip registering entirely so the
        // two never both register the same model IDs.
        if (ReplaceableSubmod.IsSupersededBy("SlimeBossBeta")) return;

        BundledSubmodLocRegistry.Register(ModId);
        VotingPoolRegistry.Register<Core.SlimeBossCardPool>(VotingPool.Slimeboss, ModId);
        HivePowerExemptRegistry.Register<SlimeModel>();
        ModPatcher.Create(ModId, Logger)
            .Add(typeof(SlimeDeathPatches))
            .Add(typeof(SlimeHoverTipPatch))
            .PatchAll();

        FormBoneRegistry.RegisterVoidForm<Core.SlimeBoss>("hat");
        FormBoneRegistry.RegisterSerpentForm<Core.SlimeBoss>("hat");
        FormBoneRegistry.RegisterReaperForm<Core.SlimeBoss>("hat");
        FormBoneRegistry.RegisterEchoForm<Core.SlimeBoss>("hat");
    }
}