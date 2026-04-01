using Downfall.Code.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Downfall.Code.Events;

public static class DownfallSubscriber
{
    private static readonly AwakenedModel AwakenedModel = ModelDb.GetById<AwakenedModel>(ModelDb.GetId<AwakenedModel>());
    private static readonly AutomatonModel AutomatonModel = ModelDb.GetById<AutomatonModel>(ModelDb.GetId<AutomatonModel>());
    public static void Subscribe()
    {
        // Tell the game: "When a run starts, call 'CollectModels' to find my hook listeners."
        ModHelper.SubscribeForRunStateHooks(DownfallMainFile.ModId, CollectModels);
        ModHelper.SubscribeForCombatStateHooks(DownfallMainFile.ModId, CollectModels2);
    }

    private static IEnumerable<AbstractModel> CollectModels2(CombatState combatState)
    {
        return [AwakenedModel, AutomatonModel];
    }

    private static IEnumerable<AbstractModel> CollectModels(RunState runState)
    {
        return [AwakenedModel, AutomatonModel];
    }
}