using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Powers;

public class ClassDefaultPower : AutomatonPowerModel
{
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        return dealer == Owner && cardSource is FunctionCard ? Amount : 0;
    }

    public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource,
        CardPlay? cardPlay)
    {
        return cardSource?.Owner.Creature == Owner && cardSource is FunctionCard ? Amount : 0;
    }
}