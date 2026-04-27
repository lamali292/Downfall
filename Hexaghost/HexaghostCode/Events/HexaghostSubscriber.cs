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
        foreach (var player in combatState.Players)
        {
            if (player.Character is not Core.Hexaghost) continue;
            foreach (var ghostflame in HexaghostModel.Wheel[player] ?? []) yield return ghostflame;
        }
    }
}