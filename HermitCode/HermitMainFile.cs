using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Utils;
using Godot;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.Patches;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;


namespace Hermit.HermitCode;

[ModInitializer(nameof(Initialize))]
public partial class HermitMainFile : Node
{
    public const string ModId = "Hermit";

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        CardExecutionRegistry.RegisterBefore(HermitCardEffectHandler.DoBeforeOnPlayInternal);
        CardExecutionRegistry.RegisterAfter(HermitCardEffectHandler.DoAfterOnPlayInternal);

        BundledSubmodLocRegistry.Register(ModId);

        ModPatcher.Create(ModId, Logger)
            .Add(typeof(DeadOnPatch))
            .Add(typeof(ShotglassLimitPatch))
            .Add(typeof(HandRefreshLayoutPatch))
            .Add(typeof(TransformShineUpdateCardPatch))
            .Add(typeof(HandChangedPatches))
            .PatchAll();


        FormBoneRegistry.RegisterVoidForm<Core.Hermit>("HEAD");
        FormBoneRegistry.RegisterSerpentForm<Core.Hermit>("Waist");
        FormBoneRegistry.RegisterReaperForm<Core.Hermit>("HEAD");
        FormBoneRegistry.RegisterEchoForm<Core.Hermit>("Waist");
    }
}