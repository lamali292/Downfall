using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Utils;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Nodes.Vfx.Forms;
using Snecko.SneckoCode.Core;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace Snecko.SneckoCode;

[ModInitializer(nameof(Initialize))]
public static class SneckoMainFile
{
    public const string ModId = "Snecko"; //At the moment, this is used only for the Logger and harmony names.

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        ScriptRegistration.Register(typeof(SneckoMainFile).Assembly);
        
        BundledSubmodLocRegistry.Register(ModId);
        CharacterOrderRegistry.Register<Core.Snecko>(70);
        MuddleListenerRegistry.Register("IntoTheSpireverse.IntoTheSpireverseCode.Utils");


        /*
        ModPatcher.Create(ModId, Logger)
            .Add(typeof(SneckoSpiritDialoguePatch))
            .Add(typeof(SneckoSpiritEntryPatch))
            .Add(typeof(SneckoSpiritGateResetPatch))
            .Add(typeof(SneckoSpiritAutoSkipPatch))
            .PatchAll();*/

        FormBoneRegistry.Register<NVoidFormVfx, Core.Snecko>("eye");
        FormBoneRegistry.Register<NSerpentFormVfx, Core.Snecko>("spine5");
        FormBoneRegistry.Register<NReaperFormVfx, Core.Snecko>("spine10");
        FormBoneRegistry.Register<NEchoFormVfx, Core.Snecko>("spine10");

        //RunManager.Instance.RunStarted += _ => SneckoSpiritGate.Reset();
    }
}