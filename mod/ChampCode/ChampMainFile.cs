using BaseLib.Utils;
using Champ.ChampCode.Cards;
using Champ.ChampCode.Core;
using Champ.ChampCode.Events;
using Champ.ChampCode.Localization;
using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Patches;
using Downfall.DownfallCode.Utils;
using Downfall.DownfallCode.Voting;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Nodes.Vfx.Forms;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace Champ.ChampCode;

[ModInitializer(nameof(Initialize))]
public static class ChampMainFile
{
    public const string ModId = "Champ"; //At the moment, this is used only for the Logger and harmony names.

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        ScriptRegistration.Register(typeof(ChampMainFile).Assembly);
        
        CustomLocTableManager.Register("champ_stances");
        CardDescriptionRegistry.Register<ChampCardModel>(DescriptionInjectionPoint.BelowMainText,
            new SkillBonusDescriptionSource());
        CardDescriptionRegistry.Register<ChampCardModel>(DescriptionInjectionPoint.BelowMainText,
            new FinisherDescriptionSource());
        ChampSubscriber.Subscribe();

        BundledSubmodLocRegistry.Register(ModId);
        CharacterOrderRegistry.Register<Core.Champ>(10);
        VotingPoolRegistry.Register<ChampCardPool>(VotingPool.Champ, ModId);

        FormBoneRegistry.Register<NVoidFormVfx, Core.Champ>("Head");
        FormBoneRegistry.Register<NSerpentFormVfx, Core.Champ>("Neck");
        FormBoneRegistry.Register<NReaperFormVfx, Core.Champ>("Neck");
        FormBoneRegistry.Register<NEchoFormVfx, Core.Champ>("Neck");
    }
}