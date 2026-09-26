using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

// "You cannot gain Block from cards for N turns." Amount = turns remaining, decremented at the start of
// each of the owner's turns and removed at 0.
public class NoBlockFromCardsPower : SlimeBossPowerModel
{
    public override decimal ModifyBlockMultiplicative(Creature target, decimal block, ValueProp props,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        return target == Owner && cardSource != null ? 0m : 1m;
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side != Owner.Side) return;
        if (Amount <= 1) await PowerCmd.Remove(this);
        else await PowerCmd.ModifyAmount(new BlockingPlayerChoiceContext(), this, -1, null, null);
    }
}
