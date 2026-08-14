using Awakened.AwakenedCode.Cards;
using Awakened.AwakenedCode.Cards.Uncommon;
using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Localization;
using Awakened.AwakenedCode.Patches;
using BaseLib.Utils;
using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Patches;
using Downfall.DownfallCode.Utils;
using Godot;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace Awakened.AwakenedCode;

[ModInitializer(nameof(Initialize))]
public partial class AwakenedMainFile : Node
{
    public const string ModId = "Awakened"; //At the moment, this is used only for the Logger and harmony names.

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        PostInitRegistry.Register(PostModelInit);
        CustomLocTableManager.Register("chants");
        CardExecutionRegistry.RegisterAfter(AwakenedCardEffectHandler.DoAfterOnPlayInternal);
        CardDescriptionRegistry.Register<AwakenedCardModel>(DescriptionInjectionPoint.BelowMainText,
            new ChantDescriptionSource());

        BundledSubmodLocRegistry.Register(ModId);
        CombatUiHooks.Register(AwakenedModel.SetupAwakenedCombatUi);
        
        ModPatcher.Create(ModId, Logger)
            .Add(typeof(PlayerCmdLoseEnergyPatch))
            .PatchAll();

        
        FormBoneRegistry.RegisterVoidForm<Core.Awakened>("Eye");
        FormBoneRegistry.RegisterSerpentForm<Core.Awakened>("Shoulder_feathers");
        FormBoneRegistry.RegisterReaperForm<Core.Awakened>("Shoulder_feathers");
        FormBoneRegistry.RegisterEchoForm<Core.Awakened>("Shoulder_feathers");
    }

    private static void PostModelInit()
    {
        CustomBundleRegistry.Register<Core.Awakened>(new CustomPackage {
            ChancePercent = 2,
            Card1 = ModelDb.Card<Caw>(),
            Card2 = ModelDb.Card<Caw>(),
            Card3 = ModelDb.Card<Caw>(),
        });
    }
}