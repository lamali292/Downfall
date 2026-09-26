using BaseLib.Utils;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Relics;

[Pool(typeof(SlimeBossRelicPool))]
public class PaperHat : SlimeBossRelicModel, IModifyDamageMultiplicative
{
    private const decimal BonusMultiplier = 1.25m;

    public PaperHat() : base(RelicRarity.Rare)
    {
        WithTip<WeakPower>();
    }

    public decimal ModifyDamageMultiplicativeCompability(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (target == null || target.GetPowerAmount<WeakPower>() <= 0) return 1m;

        var fromOwner = dealer == Owner.Creature ||
                         (dealer?.Monster is SlimeModel slime && slime.PetOwner == Owner.Creature);
        if (!fromOwner) return 1m;

        return props.IsPoweredAttack() || dealer?.Monster is SlimeModel ? BonusMultiplier : 1m;
    }
}
