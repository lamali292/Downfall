using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

// "Whenever ANY player gains Block this turn, apply 1 Weak to ALL enemies." Self-removes at end of the
// owner's turn.
public class SpreadTheFearPower : SlimeBossPowerModel
{
    public SpreadTheFearPower()
    {
        WithTip(StaticHoverTip.Block);
        WithTip<WeakPower>();
    }
    
    public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props,
        CardModel? cardSource)
    {
        if (!creature.IsPlayer) return;
        var ctx = new BlockingPlayerChoiceContext();
        await PowerCmd.Apply<WeakPower>(ctx, CombatState.HittableEnemies, Amount, Owner, null);
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side,
        IEnumerable<Creature> participants)
    {
        return participants.Contains(Owner) ? PowerCmd.Remove(this) : Task.CompletedTask;
    }
}
