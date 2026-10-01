using Collector.CollectorCode.Core;
using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Interfaces;
using Downfall.DownfallCode.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Patches;

/// <summary>
/// Pays part of a card's cost from Reserve. The split is decided by <see cref="ReservePaymentRules"/>;
/// this patch only reads the numbers off the card and applies the result.
/// X-energy cards get their Reserve merged into Energy and then run the vanilla SpendResources, so the
/// game's X plumbing (captured X value, history, hooks) sees the combined total. Reserve-only cards
/// skip the vanilla spend entirely, since they never touch Energy.
/// </summary>
[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendResources))]
internal static class ReserveSpendPatch
{
    [HarmonyPrefix]
    private static bool PayFromReserve(CardModel __instance, ref Task<(int, int)> __result)
    {
        var reserveModel = CollectorEnergy.Instance;
        var combatState = __instance.Owner.PlayerCombatState;
        if (reserveModel == null || combatState == null) return true;

        var reserveOnly = __instance is IUsesCollectorEnergyOnly;
        var costsX = __instance.EnergyCost.CostsX;
        var payment = ReservePaymentRules.Pay(combatState.Energy, reserveModel.Get(combatState),
            costsX ? 0 : __instance.EnergyCost.GetAmountToSpend(), costsX, reserveOnly);

        if (payment is { ConvertedToEnergy: true, ReserveSpent: > 0 })
            combatState.GainEnergy(payment.ReserveSpent);
        if (payment.ReserveSpent > 0)
            combatState.Reserve -= payment.ReserveSpent;
        reserveModel.RecordSpent(__instance, payment.ReserveSpent);

        // Reserve-only cards and X cards report nothing spent here; only reserve-only cards skip vanilla.
        if (!reserveOnly) return true;
        __result = Task.FromResult((0, 0));
        return false;
    }
}
