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
        return AppliesTo(cardSource) ? Amount : 0;
    }

    public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource,
        CardPlay? cardPlay)
    {
        return AppliesTo(cardSource) ? Amount : 0;
    }

    /// <summary>
    ///     Only Attack/Skill Functions actually resolve their Block/Damage through a card play.
    ///     A Function is Power-typed only when it carries Full Release (see
    ///     <see cref="FunctionCard.CalcType" />), and Full Release makes a Function's other
    ///     Encodings skip resolving on play entirely (<see cref="FunctionCard.OnPlay" />) in favor
    ///     of releasing them later through <see cref="FullReleasePower" />'s turn-start trigger,
    ///     which has no card source at all. So Power Functions must be excluded here too, or the
    ///     card would preview a bonus it can never actually deliver.
    /// </summary>
    private bool AppliesTo(CardModel? cardSource)
    {
        return cardSource is FunctionCard functionCard &&
               functionCard.Owner.Creature == Owner &&
               functionCard.Type != CardType.Power;
    }
}
