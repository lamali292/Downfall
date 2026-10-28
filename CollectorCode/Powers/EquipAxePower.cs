using Collector.CollectorCode.Core;
using Collector.CollectorCode.Events;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Powers;

public class EquipAxePower() : CollectorPowerModel, IShouldTorchheadTargetAll
{
    public bool ShouldTorchheadTargetAll(Player player) => player.Creature == Owner;
}
