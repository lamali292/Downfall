using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Powers;

public class SharePower : AutomatonPowerModel
{
    public override async Task AfterBlockGained(
        Creature creature,
        decimal amount,
        ValueProp props,
        CardModel? cardSource)
    {
        if (creature != Owner || creature.Player == null || cardSource is not FunctionCard)
            return;
        var player = Owner.Player?.OtherTeammates
            .OrderBy(e => e.Creature.Block)
            .FirstOrDefault();
        if (player == null) return;
        await CreatureCmd.GainBlock(player.Creature, Amount, BlockProps.nonCardUnpowered, null);
    }
}