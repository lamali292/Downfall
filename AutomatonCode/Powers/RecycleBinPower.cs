using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Powers;

public class RecycleBinPower : AutomatonPowerModel
{
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext ctx, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner)) return;
        var block = Owner.Player?.GetHand().Count(e => e.Type is CardType.Curse or CardType.Status);
        if (block is null or 0) return;
        Flash();
        await CreatureCmd.GainBlock(Owner, block.Value * Amount, ValueProp.Unpowered, null);
    }
}