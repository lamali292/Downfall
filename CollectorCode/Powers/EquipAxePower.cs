using Collector.CollectorCode.Core;
using Collector.CollectorCode.Events;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Powers;

public class EquipAxePower() : CollectorPowerModel, IShouldTorchheadTargetAll
{
    public bool ShouldTorchheadTargetAll(Player player) => player.Creature == Owner;
    

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (dealer?.PetOwner?.Creature != Owner || dealer.Monster is not TorchheadMonsterModel) return 0;
        return Amount;
    }
}