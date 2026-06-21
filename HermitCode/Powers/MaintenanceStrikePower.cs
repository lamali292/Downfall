using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Powers;

public sealed class MaintenanceStrikePower : HermitPowerModel
{
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        return dealer == Owner &&
               cardSource != null &&
               cardSource.Rarity == CardRarity.Basic &&
               cardSource.Tags.Contains(CardTag.Strike) &&
               props.HasFlag(ValueProp.Move)
            ? Amount
            : 0;
    }
}