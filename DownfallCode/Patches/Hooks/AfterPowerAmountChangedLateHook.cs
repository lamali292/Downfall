using Downfall.DownfallCode.Events;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterPowerAmountChanged))]
internal static class AfterPowerAmountChangedLateHook
{
    private static void Postfix(ref Task __result, ICombatState combatState, PlayerChoiceContext choiceContext,
        PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        var inner = __result;
        __result = FireHookAfter(inner, combatState, choiceContext, power, amount, applier, cardSource);
    }

    private static async Task FireHookAfter(Task inner, ICombatState combatState, PlayerChoiceContext choiceContext,
        PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        await inner;
        await MyHookUtils.Dispatch<IAfterPowerAmountChangedLate>(
            combatState, choiceContext,
            h => h.AfterPowerAmountChangedLate(choiceContext, power, amount, applier, cardSource));
    }
}
