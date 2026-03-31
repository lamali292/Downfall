using BaseLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Downfall.Code.Patches;

public static class DownfallSubscriber
{
    public static void Subscribe()
    {
        // Tell the game: "When a run starts, call 'CollectModels' to find my hook listeners."
        ModHelper.SubscribeForRunStateHooks(DownfallMainFile.ModId, CollectModels);
    }

    private static IEnumerable<AbstractModel> CollectModels(RunState runState)
    {
        // We need to yield the CharacterModel instances so they receive hooks
        return (from player in runState.Players where player.Character.ShouldReceiveCombatHooks select player.Character);
    }
}