using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Awakened;

public class ChosenVersePower : AwakenedPowerModel
{
    public CardPlay? CardPlay;

    public ChosenVersePower() : base()
    {
        WithBlock(4);
    }

    public override bool IsInstanced => true;

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
}