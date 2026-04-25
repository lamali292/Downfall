using Downfall.Code.Character;
using Downfall.Code.Core.Champ;
using Downfall.Code.Core.Hexaghost;
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
            if (stance is not ChampNoStance)
                yield return stance;
            if (player.Character is not Hexaghost) continue;
            foreach (var ghostflame in HexaghostModel.Wheel[player] ?? []) yield return ghostflame;
        }
    }
}