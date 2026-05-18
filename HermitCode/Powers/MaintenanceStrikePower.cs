using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Powers;

/// <summary>
///     Your Strikes deal X more damage.
/// </summary>
public sealed class MaintenanceStrikePower : HermitPowerModel
{
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        if (dealer != Owner) return 0m;
        if (cardSource == null) return 0m;
        if (!cardSource.Tags.Contains(CardTag.Strike)) return 0m;
        return !props.HasFlag(ValueProp.Move) ? 0m : Amount;
    }
}