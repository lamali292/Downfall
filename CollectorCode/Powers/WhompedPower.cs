using Collector.CollectorCode.Core;
using Collector.CollectorCode.Events;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Powers;

public class WhompedPower : CollectorPowerModel, IPrioritizesTorchheadTarget, IModifyDamageMultiplicative
{
    public WhompedPower() : base(PowerType.Debuff, PowerStackType.Single)
    {
    }

    public decimal ModifyDamageMultiplicativeCompability(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        return target == Owner && dealer?.Monster is TorchheadMonsterModel ? 2 : 1;
    }
}
