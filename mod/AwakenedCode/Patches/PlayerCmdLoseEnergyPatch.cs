using Awakened.AwakenedCode.Events;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Patches;

[HarmonyPatch(typeof(PlayerCmd), nameof(PlayerCmd.LoseEnergy))]
public static class PlayerCmdLoseEnergyPatch
{
    private static void Postfix(decimal amount, Player player, ref Task __result)
    {
        if (amount <= 0M || CombatManager.Instance.IsEnding)
            return;
        __result = AfterTask(__result, amount, player);
    }

    private static async Task AfterTask(Task original, decimal amount, Player player)
    {
        await original;
        var combatState = player.Creature.CombatState;
        await AwakenedHook.OnDrained(combatState, new BlockingPlayerChoiceContext(), player, (int)amount);
    }
}