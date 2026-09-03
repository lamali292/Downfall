using System.Collections;
using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Events;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class JadeRing() : CollectorRelicModel(RelicRarity.Rare), IModifyCollectorMiasmaIncrement
{
    public int ModifyCollectorMiasmaIncrement(Creature creature, int current)
    {
        
        var uniqueDebuffs = creature.Powers
            .Where(p => p.TypeForCurrentAmount == PowerType.Debuff)
            .Select(p => p.Id)
            .Distinct()
            .Count();//Check for powers that don't stack (don't double count them).
        
        return creature.Side == Owner.Creature.Side ? current : current + uniqueDebuffs;//Increases by quantity
    }
}