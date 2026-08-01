using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Utils;
using Godot;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using SlimeBoss.SlimeBossCode.Patches;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace SlimeBoss.SlimeBossCode;

[ModInitializer(nameof(Initialize))]
public partial class SlimeBossMainFile : Node
{
    public const string ModId = "SlimeBoss"; //At the moment, this is used only for the Logger and harmony names.

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        BundledSubmodLocRegistry.Register(ModId);
        ModPatcher.Create(ModId, Logger)
            .Add(typeof(PersonalHivePowerSlimePatch))
            .Add(typeof(SlimeDeathPatches))
            .Add(typeof(SlimeHoverTipPatch))
            .PatchAll();
        
        FormBoneRegistry.RegisterVoidForm<Core.SlimeBoss>("hat");
        FormBoneRegistry.RegisterSerpentForm<Core.SlimeBoss>("hat");
        FormBoneRegistry.RegisterReaperForm<Core.SlimeBoss>("hat");
    }
}