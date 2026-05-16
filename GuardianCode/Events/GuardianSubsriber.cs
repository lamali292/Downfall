using Guardian.GuardianCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Events;

public class GuardianSubsriber
{
    public static void Subscribe()
    {
        ModHelper.SubscribeForCombatStateHooks(GuardianMainFile.ModId, CollectModels2);
    }

    private static IEnumerable<AbstractModel> CollectModels2(CombatState combatState)
    {
        foreach (var player in combatState.Players)
        {
            if (player.PlayerCombatState == null) continue;
            foreach (var cards in player.PlayerCombatState.AllCards.OfType<GuardianCardModel>())
            {
                foreach (var gem in cards.Gems) yield return gem;
            }
        }
    }
}