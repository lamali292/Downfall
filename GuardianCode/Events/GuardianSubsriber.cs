using Downfall.DownfallCode.Extensions;
using Guardian.GuardianCode.Cards;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Guardian.GuardianCode.Events;

public class GuardianSubsriber
{
    public static void Subscribe()
    {
        ModHelper.SubscribeForCombatStateHooks(GuardianMainFile.ModId, CollectModels2);
        ModHelper.SubscribeForRunStateHooks(GuardianMainFile.ModId, CollectModels);
    }

    private static IEnumerable<AbstractModel> CollectModels(RunState runState)
    {
        yield return ModelDb.Singleton<GuardianModel>();
    }

    private static IEnumerable<AbstractModel> CollectModels2(CombatState combatState)
    {
        foreach (var player in combatState.Players)
        {
            foreach (var cards in player.GetAllCards().OfType<GuardianCardModel>())
            {
                foreach (var gem in cards.Gems) yield return gem;
            }
        }
    }
}
