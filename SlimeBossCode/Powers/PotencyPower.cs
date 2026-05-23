using BaseLib.Patches.Localization;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Powers;

public class PotencyPower : SlimeBossPowerModel, IAddDumbVariablesToPowerDescription, IModifySecondarySlimeEffects
{
    private int Amount2 => (Amount + 1) / 2;

    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        description.Add("Amount2", Amount2);
    }

    public int ModifySecondarySlimeEffects(int amount, SlimeModel slime)
    {
        return slime.PetOwner == Owner ? amount + Amount2 : amount;
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        return dealer?.Monster is SlimeModel slime && slime.PetOwner == Owner ? Amount : 0;
    }
}