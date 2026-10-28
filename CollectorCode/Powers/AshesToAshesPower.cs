using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Powers;

/// <summary>Deflagration: the Strength an Embered Kindling grants is raised. Using the game's power-gain hook
/// also puts the bonus into the Ember's card preview.</summary>
public class AshesToAshesPower : CollectorPowerModel
{
    public AshesToAshesPower()
    {
        WithTip<Ember>();
    }

    public override decimal ModifyPowerAmountGivenAdditive(PowerModel power, Creature giver, decimal amount,
        Creature? target, CardModel? cardSource)
    {
        return power is StrengthPower && cardSource is Ember && giver == Owner ? Amount : 0;
    }
}
