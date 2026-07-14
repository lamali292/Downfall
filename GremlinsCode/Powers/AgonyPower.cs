using Downfall.DownfallCode.Compatibility;
using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gremlins.GremlinsCode.Powers;

public class AgonyPower : GremlinsPowerModel, IModifyDamageMultiplicative
{
    public AgonyPower() : base(PowerType.Debuff)
    {
        WithVar("DamageDecrease", 0.2M);
    }

    public decimal ModifyDamageMultiplicativeCompability(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        return dealer != Owner || !props.IsPoweredAttack() ? 1 : DynamicVars["DamageDecrease"].BaseValue;
    }


    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Enemy)
            return;
        await PowerCmd.TickDownDuration(this);
    }
}