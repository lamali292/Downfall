using BaseLib.Abstracts;
using BaseLib.Extensions;
using Gremlins.GremlinsCode.Core;
using Gremlins.GremlinsCode.CustomEnums;
using Gremlins.GremlinsCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gremlins.GremlinsCode.Powers;

public class WizPower : GremlinsPowerModel, IHasSecondAmount
{
    public WizPower()
    {
        WithVar("ExtraDamage", 7);
    }

    private decimal ExtraDamage => GremlinsHook.ModifyWizExtraDamage(this, 7);

    public string GetSecondAmount()
    {
        return Amount < 3 ? "" : $"{DynamicVars["ExtraDamage"].BaseValue}";
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        UpdateExtraDamage();
        return Task.CompletedTask;
    }

    public void UpdateExtraDamage()
    {
        var a = ExtraDamage;
        var b = DynamicVars["ExtraDamage"].BaseValue;
        if (a == b) return;
        DynamicVars["ExtraDamage"].BaseValue = a;
        this.InvokeSecondAmountChanged();
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power == this) this.InvokeSecondAmountChanged();
        return Task.CompletedTask;
    }


    protected override object InitInternalData()
    {
        return new Data();
    }

    public override Task BeforeAttack(AttackCommand command)
    {
        if (Amount < 3 || command.Attacker != Owner || !command.DamageProps.IsPoweredAttack())
            return Task.CompletedTask;
        var internalData = GetInternalData<Data>();
        if (internalData.CommandToModify != null ||
            (command.ModelSource != null && command.ModelSource is not CardModel) ||
            !command.DamageProps.IsPoweredAttack())
            return Task.CompletedTask;
        internalData.CommandToModify = command;
        return Task.CompletedTask;
    }

    public override decimal DownfallModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (Amount < 3 || Owner != dealer || !props.IsPoweredAttack())
            return 0M;
        var internalData = GetInternalData<Data>();
        return (internalData.CommandToModify != null && cardSource != null &&
                cardSource != internalData.CommandToModify.ModelSource) || (internalData.CommandToModify != null &&
                                                                            internalData.CommandToModify.Attacker !=
                                                                            dealer)
            ? 0M
            : DynamicVars["ExtraDamage"].BaseValue;
    }

    public override async Task AfterAttack(PlayerChoiceContext ctx, AttackCommand command)
    {
        if (Amount < 3) return;
        var internalData = GetInternalData<Data>();
        if (command != internalData.CommandToModify)
            return;
        if (internalData.CommandToModify.ModelSource is CardModel card && card.Tags.Contains(GremlinTag.IgnoreWiz))
        {
            internalData.CommandToModify = null;
            return;
        }
        await PowerCmd.Remove<WizPower>(Owner);
    }

    protected override Task AfterRemoved(PlayerChoiceContext ctx, Creature oldOwner)
        => GremlinsHook.AfterWizConsumed(CombatState, ctx, oldOwner);

    private class Data
    {
        public AttackCommand? CommandToModify;
    }
}