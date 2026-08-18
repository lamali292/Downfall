using BaseLib.Utils;
using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Gremlins.GremlinsCode.Relics;

[Pool(typeof(GremlinsRelicPool))]
public class WoundPoker : GremlinsRelicModel
{
    public WoundPoker() : base(RelicRarity.Common)
    {
        WithDamage(4);
        WithTip<WeakPower>();
    }
    
    public override async Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner.Creature)) return;
        var combatState = Owner.Creature.CombatState;
        if (combatState == null) return;
        await CreatureCmd.Damage(ctx, combatState.HittableEnemies.Where(e => e.HasPower<WeakPower>()), DynamicVars.Damage, Owner.Creature);
    }
}