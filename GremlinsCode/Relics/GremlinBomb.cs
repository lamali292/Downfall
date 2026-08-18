using BaseLib.Utils;
using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Gremlins.GremlinsCode.Relics;

[Pool(typeof(GremlinsRelicPool))]
public class GremlinBomb : GremlinsRelicModel
{
    public GremlinBomb() : base(RelicRarity.Rare)
    {
        WithDamage(30);
    }
    
    public override async Task AfterDeath(PlayerChoiceContext ctx, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature.Monster is not GremlinsMonsterModel || creature.PetOwner != Owner) return;
        var combatState = Owner.Creature.CombatState;
        if (combatState == null) return;
        VfxCmd.PlayOnCreatureCenters(combatState.HittableEnemies, "vfx/vfx_attack_slash");
        await CreatureCmd.Damage(ctx, combatState.HittableEnemies, DynamicVars.Damage, Owner.Creature);
    }
}