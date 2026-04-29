using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Abstract;

public abstract class HookedPowerModel : CustomPowerModel
{
    
    private async Task ExecuteWithContext(Func<PlayerChoiceContext, Task> action)
    {
        if (LocalContext.NetId == null)
            throw new InvalidOperationException(
                $"Cannot execute power hook '{GetType().Name}': LocalContext.NetId is null.");
        var ctx = new HookPlayerChoiceContext(
            this,
            LocalContext.NetId.Value,
            CombatState,
            GameActionType.Combat);
        await ctx.AssignTaskAndWaitForPauseOrCompletion(action(ctx));
    }
    public sealed override Task AfterApplied(Creature? applier, CardModel? cardSource)
        => ExecuteWithContext(ctx => AfterApplied(ctx, applier, cardSource));

    protected virtual Task AfterApplied(PlayerChoiceContext ctx, Creature? applier, CardModel? cardSource)
        => Task.CompletedTask;

    public sealed override Task AfterRemoved(Creature oldOwner)
        => ExecuteWithContext(ctx => AfterRemoved(ctx, oldOwner));

    protected virtual Task AfterRemoved(PlayerChoiceContext ctx, Creature oldOwner)
        => Task.CompletedTask;

    public sealed override Task AfterCardGeneratedForCombat(CardModel card, Player? player)
        => ExecuteWithContext(ctx => AfterCardGeneratedForCombat(ctx, card, player));

    protected virtual Task AfterCardGeneratedForCombat(PlayerChoiceContext ctx, CardModel card, Player? player)
        => Task.CompletedTask;
    
    public sealed override Task AfterEnergyReset(Player player)
        => ExecuteWithContext(ctx => AfterEnergyReset(ctx, player));

    protected virtual Task AfterEnergyReset(PlayerChoiceContext ctx, Player player)
        => Task.CompletedTask;

    public sealed override Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
        => ExecuteWithContext(ctx => AfterSideTurnStart(ctx, side, combatState));

    protected virtual Task AfterSideTurnStart(PlayerChoiceContext ctx, CombatSide side, ICombatState combatState)
        => Task.CompletedTask;

}