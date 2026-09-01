using Downfall.DownfallCode.Abstract;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.DownfallCode.Patches;

// Harmony patches that work with any CardResource
[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendResources))]
internal static class GenericSpendResourcesPatch
{
    [HarmonyPrefix]
    private static bool HandleResourceSpending(CardModel __instance, ref Task<(int, int)> __result)
    {
        var player = __instance.Owner;
        if (player.PlayerCombatState == null) return true;

        foreach (var resource in CardResourceRegistry.GetAll())
            if (resource.ShouldHandleSpending(__instance))
            {
                var result = resource.HandleSpending(__instance);
                __result = Task.FromResult(result);
                return !resource.UsesResourceExclusively(__instance);
            }

        return true;
    }
}

[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))]
internal static class GenericHasEnoughResourcesPatch
{
    [HarmonyPrefix]
    private static bool HandleExclusiveResourceLogic(PlayerCombatState __instance, CardModel card,
        ref bool __result, ref UnplayableReason reason)
    {
        foreach (var resource in CardResourceRegistry.GetAll())
            if (resource.ShouldHandleResourceCheck(card) && resource.UsesResourceExclusively(card))
            {
                var check = resource.CheckResources(card);
                __result = check.hasResources;
                reason = check.reason;
                return false; // Skip original method
            }

        return true;
    }

    [HarmonyPostfix]
    private static void HandleHybridResourceLogic(PlayerCombatState __instance, CardModel card,
        ref bool __result, ref UnplayableReason reason)
    {
        if (__result) return; // Already has enough resources
        if (!reason.HasFlag(UnplayableReason.EnergyCostTooHigh)) return;

        if (!(from resource in CardResourceRegistry.GetAll()
                where resource.ShouldHandleResourceCheck(card) && !resource.UsesResourceExclusively(card)
                select resource.CheckResources(card)).Any(check => check.hasResources)) return;
        reason &= ~UnplayableReason.EnergyCostTooHigh;
        __result = reason == UnplayableReason.None;
    }
}

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
internal static class GenericResourceUiPatch
{
    private static void Postfix(NCombatUi __instance, CombatState state)
    {
        var player = LocalContext.GetMe(state);
        if (player == null) return;

        foreach (var resource in CardResourceRegistry.GetAll())
        {
            var counter = resource.CreateCounter(player);
            if (counter == null) continue;
            counter.Position = resource.UiPosition;
            counter.Scale = resource.UiScale;
            __instance.EnergyCounterContainer.AddChild(counter);
        }
    }
}