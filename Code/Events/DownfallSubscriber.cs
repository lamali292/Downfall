using Downfall.Code.Core;
using Downfall.Code.Core.Champ;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Events;

public static class DownfallSubscriber
{
    public static void Subscribe()
    {
        ModHelper.SubscribeForCombatStateHooks(DownfallMainFile.ModId, CollectModels2);
    }

    private static IEnumerable<AbstractModel> CollectModels2(CombatState combatState)
    {
        foreach (var player in combatState.Players)
        {
            var stance = ChampModel.GetStanceModel(player);
            if (stance is not NoChampStance)
                yield return stance;
            
            yield return DownfallHistory.Get(player);
        }
    }
}