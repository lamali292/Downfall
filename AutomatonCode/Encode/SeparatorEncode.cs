using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Encode;

public class SeparatorEncode() : SimpleAttackEncode(5, 2), IModifyDamageMultiplicative
{
    public decimal ModifyDamageMultiplicativeCompability(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        if (cardSource != Owner) return 1;
        return 2;
    }
}