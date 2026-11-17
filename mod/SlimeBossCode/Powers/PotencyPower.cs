using BaseLib.Patches.Localization;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Powers;

public class PotencyPower : SlimeBossPowerModel, IModifySlimeEffects
{

    // Potency can be applied either to a player (boosts every Slime that player owns) or directly to a
    // Slime's own Creature (boosts just that Slime). Both stack. Never crosses to another player's Slimes.
    private bool AppliesTo(SlimeModel slime) => slime.Creature == Owner || slime.PetOwner == Owner;

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        return dealer?.Monster is SlimeModel slime && AppliesTo(slime) ? Amount : 0;
    }

    public int ModifySlimeEffectsAdditive(SlimeModel slime)
    {
        return AppliesTo(slime) ? Amount : 0;
    }
}