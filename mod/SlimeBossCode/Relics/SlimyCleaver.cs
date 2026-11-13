using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Events;

namespace SlimeBoss.SlimeBossCode.Relics;

[Pool(typeof(SlimeBossRelicPool))]
public class SlimyCleaver : SlimeBossRelicModel, IPreventConsumeWeakRemoval
{
    public SlimyCleaver() : base(RelicRarity.Uncommon)
    {
        WithVar("UsesLeft", 2);
        WithVar("MaxUses", 2);
        WithTip<WeakPower>();
    }

    private DynamicVar UsesLeft => DynamicVars["UsesLeft"];

    public bool TryPreventWeakRemoval(Creature target, Creature consumer)
    {
        if (consumer != Owner.Creature || UsesLeft.BaseValue <= 0) return false;
        UsesLeft.BaseValue--;
        return true;
    }

    public override Task BeforeCombatStart()
    {
        UsesLeft.BaseValue = DynamicVars["MaxUses"].BaseValue;
        return Task.CompletedTask;
    }
}
