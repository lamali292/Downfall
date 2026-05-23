using Awakened.AwakenedCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Powers;

public class ChosenVersePower : AwakenedPowerModel
{
    public CardPlay? CardPlay;

    public ChosenVersePower()
    {
        WithBlock(4);
    }

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public void SetBlock(int block)
    {
        DynamicVars.Block.BaseValue = block;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player) return;
        if (cardPlay == CardPlay) return;
        if (cardPlay.Card.Type == CardType.Attack) return;
        await CardPileCmd.Draw(context, 1, cardPlay.Card.Owner);
        await CreatureCmd.GainBlock(Owner, DynamicVars.Block, null);
        Flash();
        await PowerCmd.Decrement(this);
    }

    public override async Task AfterSideTurnEndLate(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        await PowerCmd.Remove(this);
    }
}