using Awakened.AwakenedCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Awakened.AwakenedCode.Powers;

public class SpellshieldPower : AwakenedPowerModel
{
    // just hope nothing gets retained between this and the actually Flush where retain happens
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || Owner.Player == null) return;
        var a = Owner.Player.GetHand().Count(e => e.ShouldRetainThisTurn);
        for (var i = 0; i < a; i++)
        {
            await CreatureCmd.GainBlock(Owner, Amount, BlockProps.nonCardUnpowered, null);
            Flash();
        }
    }

    /*
    public override async Task AfterFlush(
        PlayerChoiceContext choiceContext,
        Player player,
        IReadOnlyCollection<CardModel> flushedCards,
        IReadOnlyCollection<CardModel> retainedCards)
    {
        if (player.Creature != Owner) return;
        foreach (var card in retainedCards.Where(e => e.ShouldRetainThisTurn))
            await CreatureCmd.GainBlock(card.Owner.Creature, Amount, BlockProps.nonCardUnpowered, null);
    }*/
}