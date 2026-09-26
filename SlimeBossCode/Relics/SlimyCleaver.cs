using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Events;

namespace SlimeBoss.SlimeBossCode.Relics;

[Pool(typeof(SlimeBossRelicPool))]
public class SlimyCleaver : SlimeBossRelicModel, IAfterConsumeEffect
{
    public SlimyCleaver() : base(RelicRarity.Uncommon)
    {
        WithVar("UsesLeft", 2);
        WithVar("MaxUses", 2);
        WithTip<WeakPower>();
    }

    private DynamicVar UsesLeft => DynamicVars["UsesLeft"];

    public async Task AfterConsumeEffect(PlayerChoiceContext ctx, Creature creature, Creature attacker)
    {
        if (attacker != Owner.Creature || UsesLeft.BaseValue <= 0) return;
        UsesLeft.BaseValue--;
        await PowerCmd.Apply<WeakPower>(ctx, creature, 1, Owner.Creature, null);
    }

    public override Task BeforeCombatStart()
    {
        UsesLeft.BaseValue = DynamicVars["MaxUses"].BaseValue;
        return Task.CompletedTask;
    }
}
