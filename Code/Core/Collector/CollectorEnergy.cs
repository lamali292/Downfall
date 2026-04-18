using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Nodes;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.Code.Core.Collector;

public class CollectorEnergy : CustomSingletonModel
{
    public CollectorEnergy() : base(true, true) { }
    private static readonly SpireField<Player, int> Current = new(() => 0);
    public static event Action<Player, int>? Changed;
    public static int  Get(Player player)               => Current[player];
    private static void Set(Player player, int amount)
    {
        var clamped = Math.Max(0, amount);
        Current[player] = clamped;
        Changed?.Invoke(player, clamped);
    }
    public static void Gain(Player player, int amount)  => Set(player, Current[player] + amount);
    public static void Spend(Player player, int amount) => Set(player, Current[player] - amount);
    public static bool CanAfford(Player player, int cost) => Current[player] >= cost;
    public static void Reset(Player player)             => Set(player, 0);
    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (card.Owner.PlayerCombatState == null) return true;
        var reserve = Get(card.Owner);
        if (reserve <= 0) return true;
        var cost = card.EnergyCost.GetWithModifiers(CostModifiers.All);
        return card.Owner.PlayerCombatState.Energy + reserve >= cost;
    }
    public override Task BeforeCombatStart()
    {
        var state = CombatManager.Instance.DebugOnlyGetState();
        if (state == null) return Task.CompletedTask;
        foreach (var player in state.Players)
            Reset(player);
        return Task.CompletedTask;
    }
    
    public override async Task AfterEnergyReset(Player player)
    {
        Changed?.Invoke(player, Get(player));
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendResources))]
static class SpendResourcesPatch
{
    [HarmonyPrefix]
    static bool HandleCollectorSpending(CardModel __instance, ref Task<(int, int)> __result)
    {
        var player = __instance.Owner;
        if (player.PlayerCombatState == null) return true;

        var reserve = CollectorEnergy.Get(player);
        if (reserve <= 0) return true;
        var cost   = __instance.EnergyCost.GetAmountToSpend();

        if (__instance is CollectorCardModel { UsesCollectorEnergyOnly: true })
        {
            if (!CollectorEnergy.CanAfford(player, cost))
                return true;

            CollectorEnergy.Spend(player, cost);
            __result = Task.FromResult((0, 0));
            return false;
        }
        var energy = player.PlayerCombatState.Energy;
        if (energy >= cost) return true;

        var deficit = cost - energy;
        var cover   = Math.Min(deficit, reserve);
        
        CollectorEnergy.Spend(player, cover);
        return true;
    }
}


[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))]
static class HasEnoughResourcesPatch
{
    [HarmonyPrefix]
    static bool HandleExclusivityLogic( PlayerCombatState __instance, CardModel card, ref bool __result, ref UnplayableReason reason) {
        if (card is not CollectorCardModel { UsesCollectorEnergyOnly: true }) return true; 
        
        var player = card.Owner;
        var reserve = CollectorEnergy.Get(player);
        var cost = card.EnergyCost.GetWithModifiers(CostModifiers.All);
        reason = UnplayableReason.None;
        if (reserve < cost)
            reason |= UnplayableReason.EnergyCostTooHigh;

        __result = reason == UnplayableReason.None;
        return false;
    }
    
    [HarmonyPostfix]
    static void HandleReserveEnergyLogic(
        PlayerCombatState __instance,
        CardModel card,
        ref bool __result,
        ref UnplayableReason reason)
    {
        if (card is CollectorCardModel { UsesCollectorEnergyOnly: true })
            return;
        if (__result) return;
        if (!reason.HasFlag(UnplayableReason.EnergyCostTooHigh)) return;
        var player = card.Owner;
        var reserve = CollectorEnergy.Get(player);
        if (reserve <= 0) return;
        var cost = card.EnergyCost.GetWithModifiers(CostModifiers.All);
        if (__instance.Energy + reserve < cost) return;
        reason  &= ~UnplayableReason.EnergyCostTooHigh;
        __result = reason == UnplayableReason.None;
    }
}



[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
static class NCombatUiActivatePatch
{
    static void Postfix(NCombatUi __instance, CombatState state)
    {
        var player = LocalContext.GetMe(state);
        if (player == null) return;

        var counter = NCollectorEnergyCounter.Create(player);
        counter.Position = new Vector2(80f, 80f);
        counter.Scale = new Vector2(0.6f, 0.6f);
        __instance.EnergyCounterContainer.AddChildSafely(counter);
    }
}

