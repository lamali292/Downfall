using Downfall.DownfallCode.Compatibility;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Powers;

public sealed class MaintenanceStrikePower : HermitPowerModel, IModifyDamageAdditive
{
    public decimal ModifyDamageAdditiveCompability(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        return dealer == Owner &&
               cardSource is { Rarity: CardRarity.Basic } &&
               cardSource.Tags.Contains(CardTag.Strike) &&
               props.IsPoweredAttack()
            ? Amount
            : 0;
    }
}