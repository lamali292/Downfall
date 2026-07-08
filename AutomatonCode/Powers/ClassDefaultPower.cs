using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Powers;

public class ClassDefaultPower : AutomatonPowerModel, IModifyDamageAdditive
{
    public decimal ModifyDamageAdditiveCompability(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer == Owner && cardSource != null) return Amount;
        return 0;
    }

    public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource,
        CardPlay? cardPlay)
    {
        return cardSource is FunctionCard && cardSource.Owner.Creature == Owner ? Amount : 0;
    }
}