using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Powers;

public class SharePower : AutomatonPowerModel
{
    protected override async Task AfterBlockGained(
        PlayerChoiceContext ctx,
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