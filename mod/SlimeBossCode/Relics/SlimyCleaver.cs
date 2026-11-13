using BaseLib.Utils;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Relics;

[Pool(typeof(SlimeBossRelicPool))]
public class SlimyCleaver : SlimeBossRelicModel
{
    public SlimyCleaver() : base(RelicRarity.Uncommon)
    {
        WithVar("HpLoss", 4);
        WithTip<WeakPower>();
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext ctx, PowerModel power, decimal amount,
        Creature? applier, CardModel? cardSource)
    {
        if (power is not WeakPower || amount <= 0 || !power.Owner.IsEnemy) return;
        // Slimes are pets of the player: a Weak they apply counts as the player's.
        if (applier != Owner.Creature && applier?.PetOwner != Owner) return;
        await CompatibilityCreatureCmd.Damage(ctx, power.Owner, DynamicVars["HpLoss"].BaseValue,
            DamageProps.nonCardHpLoss, Owner.Creature, null, null);
    }
}
