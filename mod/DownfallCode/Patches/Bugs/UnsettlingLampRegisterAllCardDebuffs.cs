using Downfall.DownfallCode.Abstract;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(UnsettlingLamp), nameof(UnsettlingLamp.BeforePowerAmountChanged))]
internal static class UnsettlingLampRegisterAllCardDebuffs
{
    [HarmonyPostfix]
    private static void Postfix(
        UnsettlingLamp __instance,
        PowerModel power,
        decimal amount,
        Creature target,
        Creature? applier,
        CardModel? cardSource)
    {
        if (cardSource is not DownfallCardModel)
            return;
        // Only act while mid-trigger on the same card the relic latched onto.
        if (__instance._triggeringCard == null || cardSource != __instance._triggeringCard)
            return;
        if (__instance._isFinishedTriggering)
            return;

        // Re-run the original's gate, minus the "first power only" bail.
        var ownerCreature = __instance.Owner.Creature;
        if (applier != ownerCreature) return;
        if (target.Side == ownerCreature.Side) return;
        if (!power.IsVisible) return;
        if (power.GetTypeForAmount(amount) != PowerType.Debuff) return;
        if (target.HasPower<ArtifactPower>()) return;

        // Register this power too, so HasDoubledTemporaryPowerSource can guard its internal.
        var doubled = __instance._doubledPowers;
        if (doubled != null && !doubled.Contains(power))
            doubled.Add(power);
    }
}