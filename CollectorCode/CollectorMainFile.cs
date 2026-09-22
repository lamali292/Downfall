using Collector.CollectorCode.Core;
using Collector.CollectorCode.Patches;
using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Patches;
using Downfall.DownfallCode.Utils;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Patches;
using Char = Collector.CollectorCode.Cards.Common.Char;
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
        PostInitRegistry.Register(PostModelInit);
        HivePowerExemptRegistry.Register<TorchheadMonsterModel>();
        CardExecutionRegistry.RegisterBefore(CollectorCardEffectHandler.DoBeforeOnPlayInternal);
        CardExecutionRegistry.RegisterAfter(CollectorCardEffectHandler.DoAfterPlayInternal);
        
        BundledSubmodLocRegistry.Register(ModId);
        
        FormBoneRegistry.RegisterVoidForm<Core.Collector>("robeback");
        FormBoneRegistry.RegisterSerpentForm<Core.Collector>("robeback");
        FormBoneRegistry.RegisterReaperForm<Core.Collector>("robeback");
        FormBoneRegistry.RegisterEchoForm<Core.Collector>("robeback");

        HarmonyLib.Harmony.DEBUG = true;
        
        ModPatcher.Create(ModId, Logger)
            .Add(typeof(AddMyPoolFilterPatch))
            .Add(typeof(NDamageNumVfxOverkillPatch))
            .Add(typeof(NMultiplayerPlayerStatePatch))
            .Add(typeof(OnPlayWrapperPlayCountPatch))
            .PatchAll();
    }
    
    
    private static void PostModelInit()
    {
        CustomBundleRegistry.Register<Core.Collector>(new CustomPackage
        {
            ChancePercent = 2,
            Card1 = ModelDb.Card<Char>(),
            Card2 = ModelDb.Card<Char>(),
            Card3 = ModelDb.Card<Char>()
        });
    }
}