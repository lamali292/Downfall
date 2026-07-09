using Downfall.DownfallCode.Compatibility;
using Gremlins.GremlinsCode.Core;
using Gremlins.GremlinsCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gremlins.GremlinsCode.Powers;

public class BangPower : GremlinsPowerModel, IModifyDamageAdditive
{
    protected override object InitInternalData()
    {
        return new Data();
    }

    public override Task BeforeAttack(AttackCommand command)
    {
        if (command.Attacker != Owner || !command.DamageProps.IsPoweredAttack())
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

    public decimal ModifyDamageAdditiveCompability(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (Owner != dealer || !props.IsPoweredAttack())
            return 0M;
        var internalData = GetInternalData<Data>();
        return (internalData.CommandToModify != null && cardSource != null &&
                cardSource != internalData.CommandToModify.ModelSource) ||
               (internalData.CommandToModify != null && internalData.CommandToModify.Attacker != dealer)
            ? 0M
            : Amount;
    }

    public override async Task AfterAttack(PlayerChoiceContext ctx, AttackCommand command)
    {
        var internalData = GetInternalData<Data>();
        if (command != internalData.CommandToModify)
            return;
        if (internalData.CommandToModify.ModelSource is CardModel card && card.Tags.Contains(GremlinTag.IgnoreWiz))
        {
            internalData.CommandToModify = null;
            return;
        }

        ;
        await PowerCmd.ModifyAmount(ctx, this, -internalData.AmountWhenAttackStarted, null, null);
        await PowerCmd.Remove<WizPower>(Owner);
    }

    private class Data
    {
        public int AmountWhenAttackStarted;
        public AttackCommand? CommandToModify;
    }
}