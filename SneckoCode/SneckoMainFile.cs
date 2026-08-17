using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Utils;
using Godot;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Runs;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Patches;
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
        CardExecutionRegistry.RegisterBefore(SneckoCardEffectHandler.DoBeforeOnPlayInternal);
        CardExecutionRegistry.RegisterAfter(SneckoCardEffectHandler.DoAfterOnPlayInternal);
        BundledSubmodLocRegistry.Register(ModId);
        
        
        ModPatcher.Create(ModId, Logger)
            .Add(typeof(SneckoSpiritDialoguePatch))
            .Add(typeof(SneckoSpiritOptionIconPatch))
            .Add(typeof(SneckoSpiritEntryPatch))
            .Add(typeof(SneckoSpiritGateResetPatch))
            .Add(typeof(SneckoSpiritAutoSkipPatch))
            .PatchAll();
        
        FormBoneRegistry.RegisterVoidForm<Core.Snecko>("eye");
        FormBoneRegistry.RegisterSerpentForm<Core.Snecko>("spine5");
        FormBoneRegistry.RegisterReaperForm<Core.Snecko>("spine10");
        FormBoneRegistry.RegisterEchoForm<Core.Snecko>("spine10");
        
        RunManager.Instance.RunStarted += _ => SneckoSpiritGate.Reset();
    }
}