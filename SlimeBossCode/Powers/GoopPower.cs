using BaseLib.Patches.Localization;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Cards.Uncommon;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.History;
using SlimeBoss.SlimeBossCode.Interfaces;

namespace SlimeBoss.SlimeBossCode.Powers;

public class GoopPower() : SlimeBossPowerModel(PowerType.Debuff), IModifyDamageAdditive
{
    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;
    
    protected override object InitInternalData()
    {
        return new Data();
    }

    public override Task BeforeAttack(AttackCommand command)
    {
        if (command.Attacker != Applier || !command.DamageProps.IsPoweredAttack())
            return Task.CompletedTask;
        var internalData = GetInternalData<Data>();
        if (internalData.CommandToModify != null ||
            (command.ModelSource != null && command.ModelSource is not CardModel))
            return Task.CompletedTask;
        internalData.CommandToModify = command;
        internalData.AmountWhenAttackStarted = Amount;
        return Task.CompletedTask;
    }

    public decimal ModifyDamageAdditiveCompability(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        if (Owner != target || dealer != Applier || !props.IsPoweredAttack())
            return 0M;
        var internalData = GetInternalData<Data>();
        return (internalData.CommandToModify != null && cardSource != null &&
                cardSource != internalData.CommandToModify.ModelSource) ||
               (internalData.CommandToModify != null &&
                internalData.CommandToModify.Attacker != dealer)
            ? 0M
            : Amount * (cardSource is IDoubleGoopBonus ? 2M : 1M);
    }

    public override async Task AfterAttack(PlayerChoiceContext ctx, AttackCommand command)
    {
        var attacker = command.Attacker;
        if (attacker == null) return;
        var internalData = GetInternalData<Data>();
        if (command != internalData.CommandToModify || command.Results.SelectMany(a => a).All(e => e.Receiver != Owner))
        {
            internalData.CommandToModify = null;
            return;
        }

        await ConsumeGoop(ctx, Owner, attacker, command);
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature,
        bool wasRemovalPrevented, float deathAnimLength)
    {
        var internalData = GetInternalData<Data>();
        var attacker = Applier;
        if (attacker == null || internalData.CommandToModify == null) return;

        await ConsumeGoop(choiceContext, creature, attacker, internalData.CommandToModify);
    }

    private async Task ConsumeGoop(PlayerChoiceContext ctx, Creature creature, Creature attacker, AttackCommand command)
    {
        var internalData = GetInternalData<Data>();
        var amount = Amount;
        var removeAmount = -internalData.AmountWhenAttackStarted;
        var newAmount = SlimeBossHook.ModifyGoopConsume(CombatState, removeAmount, out var consumes, creature, Applier);

        await SlimeBossHook.AfterModifyingGoopConsume(CombatState, consumes, creature, Applier);
        await PowerCmd.ModifyAmount(ctx, this, newAmount, null, null);

        if (command.ModelSource is IHasConsumeEffect slimeBossCardModel)
            await slimeBossCardModel.ConsumeEffect(ctx, creature, command, amount);

        internalData.CommandToModify = null;

        var entry = new ConsumeEntry(creature, amount, attacker, CombatState.RoundNumber, attacker.Side,
            CombatManager.Instance.History, CombatState.Players);
        CombatManager.Instance.History.Add(CombatState, entry);

        await SlimeBossHook.AfterConsumeEffect(CombatState, ctx, creature, attacker, amount);
    }

    private class Data
    {
        public int AmountWhenAttackStarted;
        public AttackCommand? CommandToModify;
    }
}