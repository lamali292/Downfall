using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.DownfallCode.Abstract;

public abstract class HookedPowerModel : CustomPowerModel
{
    private async Task ExecuteWithContext(Func<PlayerChoiceContext, Task> action)
    {
        if (LocalContext.NetId == null)
            throw new InvalidOperationException(
                $"Cannot execute power hook '{GetType().Name}': LocalContext.NetId is null.");
        if (Owner.IsDead) return;
        var ctx = new HookPlayerChoiceContext(
            this,
            LocalContext.NetId.Value,
            CombatState,
            GameActionType.Combat);
        await ctx.AssignTaskAndWaitForPauseOrCompletion(action(ctx));
    }

    public sealed override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        return ExecuteWithContext(ctx => AfterApplied(ctx, applier, cardSource));
    }

    protected virtual Task AfterApplied(PlayerChoiceContext ctx, Creature? applier, CardModel? cardSource)
    {
        return Task.CompletedTask;
    }

    public sealed override Task AfterRemoved(Creature oldOwner)
    {
        return ExecuteWithContext(ctx => AfterRemoved(ctx, oldOwner));
    }

    protected virtual Task AfterRemoved(PlayerChoiceContext ctx, Creature oldOwner)
    {
        return Task.CompletedTask;
    }

    public sealed override Task AfterCardGeneratedForCombat(CardModel card, Player? player)
    {
        return ExecuteWithContext(ctx => AfterCardGeneratedForCombat(ctx, card, player));
    }

    protected virtual Task AfterCardGeneratedForCombat(PlayerChoiceContext ctx, CardModel card, Player? player)
    {
        return Task.CompletedTask;
    }

    public sealed override Task AfterEnergyReset(Player player)
    {
        return ExecuteWithContext(ctx => AfterEnergyReset(ctx, player));
    }

    protected virtual Task AfterEnergyReset(PlayerChoiceContext ctx, Player player)
    {
        return Task.CompletedTask;
    }

    public sealed override Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        return ExecuteWithContext(ctx => AfterSideTurnStart(ctx, side, combatState));
    }

    protected virtual Task AfterSideTurnStart(PlayerChoiceContext ctx, CombatSide side, ICombatState combatState)
    {
        return Task.CompletedTask;
    }
    
    
    public sealed override Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
    {
        return ExecuteWithContext(ctx => AfterBlockGained(ctx, creature, amount, props, cardSource));
    }
    
    protected virtual Task AfterBlockGained(PlayerChoiceContext ctx, Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
    {
        return Task.CompletedTask;
    }
    
    public sealed override Task AfterModifyingHpLostAfterOsty()
    {
        return ExecuteWithContext(AfterModifyingHpLostAfterOsty);
    }
    
    protected virtual Task AfterModifyingHpLostAfterOsty(PlayerChoiceContext ctx)
    {
        return Task.CompletedTask;
    }
    
    public sealed override Task AfterModifyingBlockAmount(decimal modifiedAmount, CardModel? cardSource, CardPlay? cardPlay)
    {
        return ExecuteWithContext(ctx => AfterModifyingBlockAmount(ctx, modifiedAmount, cardSource, cardPlay));
    }
    
    protected virtual Task AfterModifyingBlockAmount(PlayerChoiceContext ctx, decimal modifiedAmount, CardModel? cardSource, CardPlay? cardPlay)
    {
        return Task.CompletedTask;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        return ExecuteWithContext(ctx => BeforeCardPlayed(ctx, cardPlay));
    }
    
    protected virtual Task BeforeCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }
}