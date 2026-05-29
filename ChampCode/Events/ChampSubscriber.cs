using Champ.ChampCode.Core;
using Champ.ChampCode.Stance;
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
        return combatState.Players.Select(ChampModel.GetStanceModel).Where(stance => stance is not ChampNoStance);
    }
}