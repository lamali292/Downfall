using BaseLib.Abstracts;
using BaseLib.Patches.Localization;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Cards.Uncommon;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Interfaces;

namespace SlimeBoss.SlimeBossCode.Powers;

public class GoopPower : SlimeBossPowerModel, IAddDumbVariablesToPowerDescription
{
    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;


    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        description.Add("IsApplierYou", LocalContext.IsMe(Applier));
    }

    

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
            (command.ModelSource != null && command.ModelSource is not CardModel) ||
            !command.DamageProps.IsPoweredAttack())
            return Task.CompletedTask;
        internalData.CommandToModify = command;
        internalData.AmountWhenAttackStarted = Amount;
        return Task.CompletedTask;
    }

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
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
        ;
    }

    public override async Task AfterAttack(PlayerChoiceContext ctx, AttackCommand command)
    {
        var internalData = GetInternalData<Data>();
        if (command != internalData.CommandToModify || command.Results.SelectMany(a => a).All(e => e.Receiver != Owner))
        {
            internalData.CommandToModify = null;
            return;
        }

        var amount = Amount;
        var creature = Owner;
        var removeAmount = -internalData.AmountWhenAttackStarted;
        var newAmount = SlimeBossHook.ModifyGoopConsume(CombatState, removeAmount, out var consumes, creature, Applier);
        await SlimeBossHook.AfterModifyingGoopConsume(CombatState, consumes, creature, Applier);
        await PowerCmd.ModifyAmount(ctx, this, newAmount, null, null);
        internalData.CommandToModify = null;
        if (command.ModelSource is IHasConsumeEffect slimeBossCardModel)
            await slimeBossCardModel.ConsumeEffect(ctx, creature, command, amount);
        await SlimeBossHook.AfterConsumeEffect(CombatState, ctx, creature, command, amount);
    }

    private class Data
    {
        public int AmountWhenAttackStarted;
        public AttackCommand? CommandToModify;
    }
}