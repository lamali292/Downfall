using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.DownfallCode.Abstract;

public abstract class CardResource : CustomSingletonModel
{
    private readonly SpireField<Player, int> _current;

    protected CardResource() : base(true, true)
    {
        _current = new SpireField<Player, int>(() => 0);
        CardResourceRegistry.Register(this);
    }

    public abstract string ResourceName { get; }

    // Make these optional
    public virtual Vector2 UiPosition => Vector2.One; // null = no UI
    public virtual Vector2 UiScale => Vector2.One;
    protected virtual bool ResetOnCombatStart => true; // opt-out
    protected virtual bool ResetOnTurnStart => false; // opt-in
    protected virtual bool InteractsWithEnergy => false;

    public event Action<Player, int>? Changed;

    public int Get(Player player)
    {
        return _current[player];
    }

    protected virtual void Set(Player player, int amount)
    {
        var clamped = Math.Max(0, amount);
        _current[player] = clamped;
        Changed?.Invoke(player, clamped);
    }

    public virtual void Gain(Player player, int amount)
    {
        Set(player, Get(player) + amount);
    }

    public virtual void Spend(Player player, int amount)
    {
        Set(player, Get(player) - amount);
    }

    public virtual bool CanAfford(Player player, int cost)
    {
        return Get(player) >= cost;
    }

    public virtual void Reset(Player player)
    {
        Set(player, 0);
    }

    // Only create UI if position is specified
    public virtual Control? CreateCounter(Player player)
    {
        return null;
    }

    public override Task BeforeCombatStart()
    {
        if (!ResetOnCombatStart) return Task.CompletedTask;

        var state = CombatManager.Instance.DebugOnlyGetState();
        if (state == null) return Task.CompletedTask;
        foreach (var player in state.Players)
            Reset(player);
        return Task.CompletedTask;
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        ICombatState combatState)
    {
        if (!ResetOnTurnStart) return Task.CompletedTask;
        foreach (var player in combatState.Players)
            Reset(player);
        return Task.CompletedTask;
    }

    // Only implement these if InteractsWithEnergy = true
    public virtual bool ShouldHandleSpending(CardModel card)
    {
        return InteractsWithEnergy;
    }

    public virtual bool ShouldHandleResourceCheck(CardModel card)
    {
        return InteractsWithEnergy;
    }

    public virtual bool UsesResourceExclusively(CardModel card)
    {
        return false;
    }

    // Default implementations for energy interaction
    public virtual (int energySpent, int starsSpent) HandleSpending(CardModel card)
    {
        return (0, 0);
    }

    public virtual (bool hasResources, UnplayableReason reason) CheckResources(CardModel card)
    {
        return (true, UnplayableReason.None);
    }
}

public static class CardResourceRegistry
{
    private static readonly List<CardResource> _resources = [];

    public static void Register(CardResource resource)
    {
        _resources.Add(resource);
    }

    public static IReadOnlyList<CardResource> GetAll()
    {
        return _resources;
    }

    public static T? Get<T>() where T : CardResource
    {
        return _resources.OfType<T>().FirstOrDefault();
    }
}

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