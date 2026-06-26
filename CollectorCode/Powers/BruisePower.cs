using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Powers;

// TODO : remove and use Hermit Bruise
public class BruisePower() : CollectorPowerModel(PowerType.Debuff)
{
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        return target != Owner || dealer != Applier ||!props.IsPoweredAttack() ? 0 : Amount;
    }


    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        description.Add("IsApplierYou", LocalContext.IsMe(Applier));
    }

    
    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;
}