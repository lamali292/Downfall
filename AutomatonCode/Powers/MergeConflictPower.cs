using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Powers;

public class MergeConflictPower : AutomatonPowerModel
{
    protected override async Task AfterCardGeneratedForCombat(PlayerChoiceContext ctx, CardModel card, Player? player)
    {
        await PowerCmd.Decrement(this);
        var clone = card.CreateClone();
        var pile = card.Pile?.Type;
        if (pile == null) return;
        var a = await CardPileCmd.AddGeneratedCardToCombat(clone, pile.Value, player);
        CardCmd.PreviewCardPileAdd(a);
    }


    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Side) return Task.CompletedTask;
        PowerCmd.Remove(this);
        return Task.CompletedTask;
    }
}