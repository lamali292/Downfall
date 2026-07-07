using Downfall.DownfallCode.Compatibility;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Powers;

public class UnyieldingPower : HermitPowerModel, IModifyDamageMultiplicative
{
    public UnyieldingPower()
    {
        WithVar("DamageDecrease", 0.5m);
        WithTip<VulnerablePower>();
    }
     
    
    public decimal ModifyDamageMultiplicativeCompability(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        return target != Owner || !props.IsPoweredAttack() || dealer == null || !target.HasPower<VulnerablePower>() ? 1M : DynamicVars["DamageDecrease"].BaseValue;
    }
    
    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Enemy)
            return;
        await PowerCmd.Decrement(this);
    }

}