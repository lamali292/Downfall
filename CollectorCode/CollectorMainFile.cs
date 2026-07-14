using System.Reflection;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode;
using Downfall.DownfallCode.Localization;
using Downfall.DownfallCode.Utils;
using Godot;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace Collector.CollectorCode;

[ModInitializer(nameof(Initialize))]
public partial class CollectorMainFile : Node
{
    public const string ModId = "Collector"; //At the moment, this is used only for the Logger and harmony names.

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        CardExecutionRegistry.RegisterBefore(CollectorCardEffectHandler.DoBeforeOnPlayInternal);
        
        BundledSubmodLocRegistry.Register(ModId);
        
        RunHooks.OnNewRunPerPlayer(player =>
        {
            EssenceModel.ClearEssence(player);
            CollectiblesModel.ClearCollectibles(player);
            if (player.Character is Core.Collector)
                EssenceModel.AddEssence(player, 5);
        });
    }
}