using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Utils;
using Downfall.DownfallCode.Voting;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using SlimeBoss.SlimeBossCode.Patches;
using SlimeBoss.SlimeBossCode.Slimes;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace SlimeBoss.SlimeBossCode;

/// <summary>
/// NOT a mod entry point. SlimeBoss is an internal submod (ADR 0003): its own assembly
/// (<c>SlimeBoss.dll</c>) for code-separation, but no manifest/ModId of its own, so the game's
/// mod loader never discovers or calls this type directly - it's called explicitly by
/// <see cref="Downfall.DownfallCode.DownfallMainFile.Initialize"/>, which also owns the
/// submod-supersession guard gating this call.
/// </summary>
public static class SlimeBossMainFile
{
    public const string ModId = "SlimeBoss"; //At the moment, this is used only for the Logger and harmony names.

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
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