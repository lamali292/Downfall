using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Patches;
using Downfall.DownfallCode.Utils;
using Hermit.HermitCode.Cards.Uncommon;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.Patches;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;


namespace Hermit.HermitCode;

[ModInitializer(nameof(Initialize))]
public static class HermitMainFile
{
    public const string ModId = "Hermit";

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        PostInitRegistry.Register(PostModelInit);
        CardExecutionRegistry.RegisterBefore(HermitCardEffectHandler.DoBeforeOnPlayInternal);
        CardExecutionRegistry.RegisterAfter(HermitCardEffectHandler.DoAfterOnPlayInternal);

        BundledSubmodLocRegistry.Register(ModId);

        ModPatcher.Create(ModId, Logger)
            .Add(typeof(DeadOnPatch))
            .Add(typeof(HandRefreshLayoutPatch))
            .Add(typeof(TransformShineUpdateCardPatch))
            .Add(typeof(HandChangedPatches))
            .PatchAll();


        FormBoneRegistry.RegisterVoidForm<Core.Hermit>("HEAD");
        FormBoneRegistry.RegisterSerpentForm<Core.Hermit>("Waist");
        FormBoneRegistry.RegisterReaperForm<Core.Hermit>("HEAD");
        FormBoneRegistry.RegisterEchoForm<Core.Hermit>("Waist");
    }

    private static void PostModelInit()
    {
        CustomBundleRegistry.Register<Core.Hermit>(new CustomPackage
        {
            ChancePercent = 2,
            Card1 = ModelDb.Card<CursedWeapon>(),
            Card2 = ModelDb.Card<CursedWeapon>(),
            Card3 = ModelDb.Card<CursedWeapon>()
        });
    }
}