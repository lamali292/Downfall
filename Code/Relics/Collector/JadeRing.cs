using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Downfall.Code.Relics.Collector;

[Pool(typeof(CollectorRelicPool))]
public class JadeRing : CollectorRelicModel, IModifyCollectorDoomDamage
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public int ModifyCollectorDoomDamage(Creature creature, int current)
    {
        return creature.Side == Owner.Creature.Side ? current : current + 6;
    }
}