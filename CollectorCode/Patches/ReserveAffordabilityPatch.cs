using Collector.CollectorCode.Core;
using Collector.CollectorCode.Interfaces;
using Downfall.DownfallCode.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Patches;

/// <summary>
/// Lets Reserve count towards "can the player afford this card". The decision is
/// <see cref="ReservePaymentRules.CanAfford"/>; this patch only reads the numbers and sets the result.
/// </summary>
[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))]
internal static class ReserveAffordabilityPatch
{
    private static bool CanAfford(CardModel card, bool reserveOnly)
    {
        var reserveModel = CollectorEnergy.Instance;
        if (reserveModel == null) return false;
        var owner = card.Owner;
        var energy = owner.PlayerCombatState?.Energy ?? 0;
        return ReservePaymentRules.CanAfford(energy, reserveModel.Get(owner), card.EnergyCost.Modified, reserveOnly);
    }

    /// <summary>Reserve-only cards ignore Energy entirely, so the vanilla check is replaced.</summary>
    [HarmonyPrefix]
    private static bool CheckReserveOnlyCards(CardModel card, ref bool __result, ref UnplayableReason reason)
    {
        if (CollectorEnergy.Instance == null || card is not IUsesCollectorEnergyOnly) return true;

        __result = CanAfford(card, true);
        reason = __result ? UnplayableReason.None : UnplayableReason.EnergyCostTooHigh;
        return false;
    }

    /// <summary>Other cards: if vanilla only failed on Energy but Energy + Reserve is enough, allow the play.</summary>
    [HarmonyPostfix]
    private static void CheckEnergyPlusReserve(CardModel card, ref bool __result, ref UnplayableReason reason)
    {
        if (__result) return;
        if (!reason.HasFlag(UnplayableReason.EnergyCostTooHigh)) return;
        if (CollectorEnergy.Instance == null || card is IUsesCollectorEnergyOnly) return;
        if (!CanAfford(card, false)) return;

        reason &= ~UnplayableReason.EnergyCostTooHigh;
        __result = reason == UnplayableReason.None;
    }
}
