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
    //todo gives block to the character with the least block
    protected override async Task AfterBlockGained(
        PlayerChoiceContext ctx, 
        Creature creature, 
        decimal amount, 
        ValueProp props,
        CardModel? cardSource)
    {
        if (creature != Owner || creature.Player == null || cardSource is not FunctionCard)
            return;
        var teammate =
            creature.Player.RunState.Rng.CombatTargets.NextItem(CombatState.GetTeammatesOf(Owner)
                .Where(e => e.IsAlive));
        if (teammate == null) return;
        await CreatureCmd.GainBlock(teammate, Amount, ValueProp.Unpowered, null);
    
    }
}