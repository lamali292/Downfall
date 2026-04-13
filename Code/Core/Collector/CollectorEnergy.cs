using BaseLib.Abstracts;
using BaseLib.Utils;
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
    public override async Task AfterEnergyReset(Player player)
    {
        Changed?.Invoke(player, Get(player));
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendResources))]
static class SpendResourcesPatch
{
    static void Prefix(CardModel __instance)
    {
        var player = __instance.Owner;
        if (player?.PlayerCombatState == null) return;

        var reserve = CollectorEnergy.Get(player);
        if (reserve <= 0) return;

        var energy = player.PlayerCombatState.Energy;
        var cost   = __instance.EnergyCost.GetAmountToSpend();
        if (energy >= cost) return;

        var deficit = cost - energy;
        var cover   = Math.Min(deficit, reserve);
        
        CollectorEnergy.Spend(player, cover);
    }
}

[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))]
static class HasEnoughResourcesPatch
{
    static void Postfix(
        PlayerCombatState __instance,
        CardModel card,
        ref bool __result,
        ref UnplayableReason reason)
    {
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

        var counter = new NCollectorEnergyCounter();
        counter.Initialize(player);
        __instance.EnergyCounterContainer.AddChildSafely(counter);
    }
}

