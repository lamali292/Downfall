using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Events;

public static class ChampSubscriber
{
    public static void Subscribe()
    {
        ModHelper.SubscribeForCombatStateHooks(ChampMainFile.ModId, CollectModels2);
    }

    private static IEnumerable<AbstractModel> CollectModels2(CombatState combatState)
    {
        foreach (var player in combatState.Players)
        {
            var stance = ChampModel.GetStanceModel(player);
            if (stance is not ChampNoStance)
                yield return stance;
        }
    }
}