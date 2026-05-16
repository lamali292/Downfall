using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Events;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class JadeRing() : CollectorRelicModel(RelicRarity.Uncommon), IModifyCollectorDoomDamage
{
    public int ModifyCollectorDoomDamage(Creature creature, int current)
    {
        return creature.Side == Owner.Creature.Side ? current : current + 6;
    }
}