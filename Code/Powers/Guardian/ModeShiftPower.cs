using Downfall.Code.Abstract;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Powers.Guardian;

public class ModeShiftPower : GuardianPowerModel, IHasSecondAmount
{
    public ModeShiftPower()
    {
        WithVar("CurrentLimit", 20);
        WithVar("MaxLimit", 50);
        WithVar("Increase", 10);
        WithBlock(16);
    }

    public override bool ShouldRemoveDueToZero => false;
    public override bool AllowNegative => true;

    public string GetSecondAmount()
    {
        return $"{DynamicVars["CurrentLimit"].BaseValue}";
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target,
        DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || Owner.Player == null) return;
        var a = result.UnblockedDamage;
        if (a <= 0) return;
        var m = await PowerCmd.ModifyAmount(this, -a, dealer, null);
        if (m > 0) return;
        await Reset();
    }


    public async Task Reset()
    {
        if (Owner.Player == null) return;
        await CreatureCmd.GainBlock(Owner, DynamicVars.Block, null);
        var a = Owner.GetPowerAmount<DefensiveModePower>();
        var g = a == 0 && CombatState.CurrentSide == CombatSide.Enemy ? 2 : 1;
        await PowerCmd.Apply<DefensiveModePower>(Owner, g, Owner, null);
        DynamicVars["CurrentLimit"].BaseValue =
            Math.Min(DynamicVars["CurrentLimit"].BaseValue + DynamicVars["Increase"].BaseValue,
                DynamicVars["MaxLimit"].BaseValue);
        await PowerCmd.Apply(this, Owner, DynamicVars["CurrentLimit"].BaseValue, Owner, null);
    }
}