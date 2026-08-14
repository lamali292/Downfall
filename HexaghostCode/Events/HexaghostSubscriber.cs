using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Events;

public static class HexaghostSubscriber
{
    public static void Subscribe()
    {
        ModHelper.SubscribeForCombatStateHooks(HexaghostMainFile.ModId, CollectModels2);
    }

    private static IEnumerable<AbstractModel> CollectModels2(CombatState combatState)
    {
        return combatState.Players.SelectMany(player => HexaghostModel.Wheel.Get(player) ?? []);
    }
}