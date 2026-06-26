using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Powers;

public class FeelMyPainPower : CollectorPowerModel
{
    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card,
        bool causedByEthereal)
    {
        if (card.Owner.Creature != Owner) return;
        var creature = CombatState.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
        if (creature == null) return;
        await CreatureCmd.Damage(ctx, creature, Amount,
            ValueProp.Unblockable | ValueProp.Unpowered, Owner, null);
        Flash();
    }
}