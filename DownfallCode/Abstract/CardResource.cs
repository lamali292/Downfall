using BaseLib.Abstracts;
using Downfall.DownfallCode.Core;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Abstract;

public abstract class CardResource : CustomSingletonModel
{
    private readonly PlayerField<int> _current;

    protected CardResource() : base(HookType.Combat)
    {
        _current = new PlayerField<int>(() => 0);
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
        IReadOnlyList<Creature> participants,
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