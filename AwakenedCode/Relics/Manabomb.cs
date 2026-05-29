using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Relics;

[Pool(typeof(AwakenedRelicPool))]
public class Manabomb : AwakenedRelicModel
{
    public Manabomb() : base(RelicRarity.Shop)
    {
        this.WithTip<ManaburnPower>();
    }

    public override async Task AfterDeath(PlayerChoiceContext ctx, Creature creature, bool wasRemovalPrevented,
        float deathAnimLength)
    {
        if (wasRemovalPrevented) return;
        var manaburn = creature.GetPowerAmount<ManaburnPower>();
        if (manaburn == 0) return;
        var target = Owner.Creature.CombatState?.HittableEnemies
            .Where(c => c != creature)
            .TakeRandom(1, Owner.RunState.Rng.CombatTargets).FirstOrDefault();
        if (target == null) return;
        var a = await PowerCmd.Apply<ManaburnPower>(ctx, target, manaburn, Owner.Creature, null);
        if (a == null) return;
        await a.OnDrained(ctx, Owner, 1);
    }
}