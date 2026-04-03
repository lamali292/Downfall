using Downfall.Code.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Downfall.Code.Events;

public static class DownfallSubscriber
{
    private static readonly AwakenedModel
        AwakenedModel = ModelDb.GetById<AwakenedModel>(ModelDb.GetId<AwakenedModel>());

    private static readonly AutomatonModel AutomatonModel =
        ModelDb.GetById<AutomatonModel>(ModelDb.GetId<AutomatonModel>());

    private static readonly GremlinsModel
        GremlinsModel = ModelDb.GetById<GremlinsModel>(ModelDb.GetId<GremlinsModel>());

    private static readonly GremlinsRunModel GremlinsRunModel =
        ModelDb.GetById<GremlinsRunModel>(ModelDb.GetId<GremlinsRunModel>());

    private static readonly ChampModel ChampModel = ModelDb.GetById<ChampModel>(ModelDb.GetId<ChampModel>());

    private static readonly SlimeBossModel SlimeBossModel =
        ModelDb.GetById<SlimeBossModel>(ModelDb.GetId<SlimeBossModel>());

    public static void Subscribe()
    {
        // Tell the game: "When a run starts, call 'CollectModels' to find my hook listeners."
        ModHelper.SubscribeForRunStateHooks(DownfallMainFile.ModId, CollectModels);
        ModHelper.SubscribeForCombatStateHooks(DownfallMainFile.ModId, CollectModels2);
    }

    private static IEnumerable<AbstractModel> CollectModels2(CombatState combatState)
    {
        return [AwakenedModel, AutomatonModel, GremlinsModel, ChampModel, SlimeBossModel];
    }

    private static IEnumerable<AbstractModel> CollectModels(RunState runState)
    {
        return [AwakenedModel, AutomatonModel, GremlinsRunModel, ChampModel, SlimeBossModel];
    }
}