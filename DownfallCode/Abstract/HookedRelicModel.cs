using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.DownfallCode.Abstract;

public abstract class HookedRelicModel : CustomRelicModel
{
    private Task ExecuteWithContext(Func<PlayerChoiceContext, Task> action)
    {
        if (LocalContext.NetId == null || Owner.Creature.CombatState == null)
            return action(new ThrowingPlayerChoiceContext());

        if (Owner.Creature.IsDead) return Task.CompletedTask;
        var ctx = new HookPlayerChoiceContext(
            this,
            LocalContext.NetId.Value,
            Owner.Creature.CombatState,
            GameActionType.Combat);
        return ctx.AssignTaskAndWaitForPauseOrCompletion(action(ctx));
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

    public sealed override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        return ExecuteWithContext(ctx => AfterSideTurnStart(ctx, side, participants, combatState));
    }

    protected virtual Task AfterSideTurnStart(PlayerChoiceContext ctx, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        return Task.CompletedTask;
    }


    public sealed override Task AfterBlockGained(Creature creature, decimal amount, ValueProp props,
        CardModel? cardSource)
    {
        return ExecuteWithContext(ctx => AfterBlockGained(ctx, creature, amount, props, cardSource));
    }

    protected virtual Task AfterBlockGained(PlayerChoiceContext ctx, Creature creature, decimal amount, ValueProp props,
        CardModel? cardSource)
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

    public sealed override Task AfterModifyingBlockAmount(decimal modifiedAmount, CardModel? cardSource,
        CardPlay? cardPlay)
    {
        return ExecuteWithContext(ctx => AfterModifyingBlockAmount(ctx, modifiedAmount, cardSource, cardPlay));
    }

    protected virtual Task AfterModifyingBlockAmount(PlayerChoiceContext ctx, decimal modifiedAmount,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        return Task.CompletedTask;
    }

    public sealed override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        return ExecuteWithContext(ctx => BeforeCardPlayed(ctx, cardPlay));
    }

    protected virtual Task BeforeCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }

    public sealed override Task AfterModifyingCardPlayResultPileOrPosition(CardModel card, PileType pileType,
        CardPilePosition position)
    {
        return ExecuteWithContext(ctx => AfterModifyingCardPlayResultPileOrPosition(ctx, card, pileType, position));
    }

    protected virtual Task AfterModifyingCardPlayResultPileOrPosition(PlayerChoiceContext card, CardModel pileType,
        PileType position, CardPilePosition cardPilePosition)
    {
        return Task.CompletedTask;
    }

    public sealed override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        return ExecuteWithContext(ctx => AfterCardChangedPiles(ctx, card, oldPileType, clonedBy));
    }

    protected virtual Task AfterCardChangedPiles(PlayerChoiceContext card, CardModel oldPileType, PileType clonedBy,
        AbstractModel? abstractModel)
    {
        return Task.CompletedTask;
    }
}